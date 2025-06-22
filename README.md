# Unity車制御プロジェクト

## 概要
Unityを使って自動運転を実施する。

## 主要なファイルと役割


- **CaptureAndSend.cs**  (Automonous_unity\Assets\Scripts\CaptureAndSend.cs)
  カメラで画像を取得し、サーバーにデータを送信する。

- **python_server.py**(python_server.py")  
  Flaskを使ったシンプルなWebサーバーを立てる。  
  Unityから送信された画像を受け取り、画像処理を実行し、結果を返す。

- **CarControl.cs**  (Automonous_unity\Assets\Scripts\CarControl.cs)
  画像処理結果を基に、車の動き（直進、左折、右折）を制御する。

## 使い方
　1. Flaskサーバーを起動
　    (python server.pyを実行)
　2. Unityから画像送信
　3. サーバー側で画像処理が行われ、結果を返す
