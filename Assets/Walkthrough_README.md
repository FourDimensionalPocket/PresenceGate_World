# ワールドの歩き方

`Assets/Scenes/SampleScene.unity` を開いて Unity の Play を押し、Game 画面をクリックしてください。

- WASD または矢印キー：移動
- マウス：視点移動
- Shift：走る
- Space：ジャンプ
- Esc：マウス操作を解除。Game 画面をクリックすると再開

床・壁・ブロックには衝突判定があります。左側の階段は歩いて上れます。
万一ワールドの下へ落ちた場合は、開始地点に戻ります。

`Player` の `First Person Walker` で歩行速度・走行速度・ジャンプの高さ・マウス感度を調整できます。
他のワールドへ移す際は Player をコピーし、足元を床の少し上に配置してください。
床や壁のオブジェクトには Collider を付けてください。

初期デモを再作成する場合は `Tools > Pocket World > Create Walkthrough Demo` を使用できます。
Player が既にあるシーンでは重複作成しません。

現段階は Unity Editor での操作確認用です。ブラウザーへのビルドと公開は今後の工程です。
