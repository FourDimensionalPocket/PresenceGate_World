# 四次元ポケット / YojigenPocket

Unityで3D空間を歩き回る体験を制作するプロジェクトです。
将来のブラウザー対応とRaspberry Piからの操作を検討しています。

## 開発環境

- Unity **6000.2.6f2（Unity 6.2）**
- Input System **1.14.2**（必要なパッケージはUnityが復元します）

## 起動方法

1. このリポジトリをcloneします。
2. Unity Hubの「追加」からリポジトリのフォルダーを登録します。
3. 指定バージョンのUnityで開き、初回のインポートとパッケージの復元を待ちます。
4. `Assets/Scenes/SampleScene.unity` を開きます。
5. Playを押し、Game画面をクリックします。

## 操作

| 操作 | キー |
| --- | --- |
| 移動 | WASD / 矢印キー |
| 視点移動 | マウス |
| 走る | Shift |
| ジャンプ | Space |
| マウスを解除 | Esc |

## 初期ワールド

衝突判定のある床・壁・障害物・階段と、一人称プレイヤーを配置しています。
落下した場合は開始地点へ戻ります。

- シーン：`Assets/Scenes/SampleScene.unity`
- プレイヤー制御：`Assets/Scripts/FirstPersonWalker.cs`
- 初期デモ作成：`Assets/Editor/WalkthroughSetup.cs`
- 操作と調整方法：`Assets/Walkthrough_README.md`

## 現在の対応範囲

Unity Editorでの制作・確認を行う段階です。コンパイル・再生画面・床への接地は確認済みです。
移動・ジャンプなどの詳細な操作検証、Webビルド、Raspberry Pi実機での動作確認は未完了です。

Raspberry PiでUnity自体を実行するか、別のPCで動かすUnityへ操作信号を送るかは未決定です。
実行構成と対象機種を決めてから、入力連携と必要な軽量化を進めます。

## バージョン管理

`Assets/`（`.meta`を含む）、`Packages/`、`ProjectSettings/`を管理します。
Unityが自動生成する`Library/`、`Temp/`、`Logs/`、個人設定やビルド出力は管理対象外です。
アセットの移動・名前変更はUnity内で行い、対応する`.meta`も一緒にコミットしてください。
