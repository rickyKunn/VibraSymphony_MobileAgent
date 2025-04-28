# VibraSymphony_MobileAgent
VibraSymphonyのモバイル端末（Android）用クライアントアプリケーションです。  
音楽データ送信とリアルタイム振動制御を通じて、VibraSymphony_Coreとのインタラクティブな体験を実現します。

---

## 本リポジトリについて

本リポジトリは、チーム開発プロジェクト「VibraSymphony」のモバイル端末向けクライアント部分を構成するアプリケーションです。  
Androidデバイスを通じて、音楽データの送信およびCore側からの解析結果受信、バイブレーション制御を行います。

プロジェクトの詳細については、以下の紹介用リポジトリをご参照ください：  
**[VibraSymphony プロジェクト紹介ページ](https://github.com/rickyKunn/VibraSymphony_Description)**

---

## 連携プロジェクトについて（依存関係）

本リポジトリは、 **別リポジトリ「VibraSymphony_Core」と連携して使用することを前提としています。**

### 関連リポジトリ

**[VibraSymphony_Core](https://github.com/rickyKunn/VibraSymphony_Core)**

---

### 役割と連携内容

**VibraSymphony_MobileAgent** は、以下の役割を担います：

- Androidデバイスから MP3 ファイルを TCP/IP を通じて Core に送信
- Core側での再生・解析により検出されたドラム・ベース等の音楽成分に対して、OSC信号を受信
- 受信したタイミングに応じて Android デバイスのバイブレーションをリアルタイム制御

これにより、音楽とデバイスの物理的なインタラクションが可能となります。

---

## 利用手順

1. Android 実機上で **VibraSymphony_MobileAgent(以下、「MobileAgent」と略記)** を起動します（Unityフォルダを実機にビルド・インストール)

2. Unity エディタ(またはOculus Quest)で **VibraSymphony_Core(以下、「Core」と略記)** を開き、StartSceneを再生(Oculusの場合、アプリを起動)

3. **MobileAgent** を起動し、`Main` ボタンを選択

4. **MobileAgent** で画面左下に表示された `ID` を **Core** の `Devices` を追加し入力

5. **Core側** で `GO!!` ボタンを選択し、接続を確立

6. **MobileAgent側** で `Pick Music` ボタンを選択し、お好みの曲を選択(Androidのダウンロードフォルダに入っている曲の一覧が表示されます)

(手順5、6の順序はどちらでも問題ありません)

7. 曲の送受信が完了すると、自動的に連携再生が開始されます。

---

### 通信に関する重要な注意点

- 本システムは、**CoreとMobileAgentが同一Wi-Fiネットワーク上にあることが必須です。**
- 使用している TCP/IP や OSC プロトコルが **制限されているネットワーク（例：学内ネットワーク、ゲストWi-Fiなど）では使用できません。**
- 通信状況が不安定な環境では、タイムラグや振動の遅延が発生する可能性があります。**良好な通信環境での使用を推奨します。**
- 何か不明な点、エラー等が発生した場合、**kobayashiritsuki@gmail.com**までご連絡ください。

---

## 実行ファイル（.apk）について

本リポジトリには、ビルド済みの実行ファイルも同梱されています。

 **Builds/VibraSymphony_MobileAgent_v1.0.apk**

- Android端末にインストールすることで、Unity環境なしでも動作確認が可能です。
- ビルド環境：Unity 6000.0.47f1 / Android API Level 30以上推奨
- インストールには「提供元不明のアプリを許可」設定が必要な場合があります。

---

## ライセンスおよびクレジット

このリポジトリは、ポートフォリオおよび選考目的での閲覧専用として公開しています。

- 商用利用は禁止です  
- 改変や再配布は禁止です  
- 閲覧および参照は自由ですが、許可なく他用途に使用しないでください

その他の目的での使用を希望される場合は、下記までご連絡ください。  
**kobayashiritsuki@gmail.com**

---

© 2025 小林立樹  
© Unity Technologies Japan. All rights reserved.
