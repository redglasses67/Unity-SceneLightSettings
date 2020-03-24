# Unity-SceneLightSettings

UnityのLightingウィンドウ内の設定と
シーン内のLight・LightProbeGroup・ReflectionProbeのデータを
Export＆ImportできるようにするEditor拡張です。

別のシーンにLightingの設定をコピーしたい場合や、
Lightmapのテストベイク用設定・本番用設定を切り替えたい場合に、
このツールを使うと便利です。

チェックボックスでImportするデータをグループごとに選ぶことも可能です。

動作確認は Unity2017.4 ～ Unity2019.2 で行っています。

●使い方
ファイルをUnityのプロジェクトにImportしてもらうと、
Unityのメニューの Tools > Scene Light Setting をクリックしてもらうとこちらに添付しているようなウィンドウが開きますので、
Lighting設定などをExportしたいシーンを開いてExportボタンを押して下さい。
Importしたいシーンを開いた状態で、先程出力されたデータをImport File Pathのところに設定して、Importボタンを押して下さい。
