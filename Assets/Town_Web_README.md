# Town_Sunny をWebで動かす

1. Unity HubでUnity 6000.2.6f2に **Web Build Support** を追加します。
2. Unityで `Tools > Pocket World > Build Town for Web` を選び、空の出力フォルダーを指定します。
3. ビルドされたフォルダーをHTTPサーバーで配信し、ブラウザーで開きます。`index.html`を直接開く方法では動かない場合があります。
4. 画面をクリックして、WASDまたは矢印キーで移動、マウスで視点操作します。Shiftで走り、Spaceでジャンプ、Escでマウスを解除します。

Town_SunnyをUnityで再生した際も、プレイヤーがなければ自動的に作られます。すでにプレイヤーを配置した場合は自動生成しません。
ブラウザーの対応状況やRaspberry Piでの動作は実機での確認が必要です。
