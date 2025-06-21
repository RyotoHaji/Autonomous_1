from flask import Flask, request, jsonify
import cv2
import numpy as np

app = Flask(__name__)

# 最初は何も結果がないのでNoneに
last_result = None

@app.route('/upload', methods=['POST'])
def upload():
    global last_result
    print("画像受信開始")
    image_bytes = request.data
    img_array = np.frombuffer(image_bytes, np.uint8)
    img = cv2.imdecode(img_array, cv2.IMREAD_COLOR)
    # 画像保存
    cv2.imwrite('test_image.jpg', img)
    # 画像処理
    left_x, center_x, right_x = process_image(img)
    width_pixels = img.shape[1]

    # 結果を更新
    last_result = {
        'left': left_x,
        'center': center_x,
        'right': right_x,
        'width': width_pixels
    }
    return jsonify(last_result)

@app.route('/getresult', methods=['GET'])
def get_result():
    global last_result
    if last_result is None:
        return jsonify({'error': '結果がありません'}), 404
    return jsonify(last_result)

# 画像処理関数はそのまま
def process_image(img):
    # ... 既存の処理
    # 例:
    hsv = cv2.cvtColor(img, cv2.COLOR_BGR2HSV)
    # 赤色と白色の範囲
    lower_red1 = np.array([0, 100, 100])
    upper_red1 = np.array([10, 255, 255])
    lower_red2 = np.array([160, 100, 100])
    upper_red2 = np.array([179, 255, 255])
    mask_red1 = cv2.inRange(hsv, lower_red1, upper_red1)
    mask_red2 = cv2.inRange(hsv, lower_red2, upper_red2)
    mask_red = cv2.bitwise_or(mask_red1, mask_red2)

    lower_white = np.array([0, 0, 200])
    upper_white = np.array([179, 20, 255])
    mask_white = cv2.inRange(hsv, lower_white, upper_white)

    red_center = detect_line_center(mask_red)
    white_center = detect_line_center(mask_white)

    if red_center is None:
        red_center = 0
    if white_center is None:
        white_center = 0
    
    center_x = (red_center + white_center) // 2
    return red_center, center_x, white_center

def detect_line_center(mask):
    contours, _ = cv2.findContours(mask, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
    if len(contours) == 0:
        return None
    max_contour = max(contours, key=cv2.contourArea)
    M = cv2.moments(max_contour)
    if M['m00'] == 0:
        return None
    cx = int(M['m10'] / M['m00'])
    return cx

if __name__ == "__main__":
    print("サーバ起動します")
    app.run(host='0.0.0.0', port=5000)