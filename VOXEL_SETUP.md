# ブロックワールド（簡易マイクラ風）動作確認

## 目的
- マップの自動生成（起動時にブロック地形を生成）
- 4種類以上のブロックを、自由に設置/削除

## 使い方（PlayScene1想定）
1. Unityでプロジェクトを開く
2. `Assets/Scenes/PlayScene1.unity` を開く
3. Hierarchyの `Managers` に付いている `BlockManager` を確認
   - `blockPrefab_Grass / blockPrefab_Stone / blockPrefab_Wood` が割り当て済みであること
   - `blockPrefab_Sand` は未設定でもOK（未設定時は実行時に砂色のCubeを自動生成）
4. 再生（Play）

## 操作
- ブロック設置: ブロックにマウスを合わせて左クリック
  - 面の向き（クリックした面の法線）に沿って隣マスへ設置します
- ブロック削除: ブロックにマウスを合わせて右クリック
- 設置ブロック切替: 数字キー
  - `1`: Wood
  - `2`: Stone
  - `3`: Grass
  - `4`: Sand

## 調整したい場合
- 地形生成パラメータは `Assets/Script/BlockManager.cs` の Inspector から変更できます
  - `mapWidth`, `mapDepth`, `maxHeight`, `noiseScale`, `seed`, `seaLevel`
