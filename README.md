# （WIP）Mes : light markup language for Japanese Scenario.

Mesは日本語シナリオの記述に特化した軽量記述言語です。  
現在、開発途中です。  

詳細は公式HPを参照してください。

https://sites.google.com/view/mesdoc


## Demo

(TDB)

### Other

(TDB)

## Build


### class lib
```
dotnet build
```

## Sample code for Using Mes library

MesBuilderクラスを使ってMesテキストをビルド（parse）するパターンが推奨です。

```
using Mes;

namespace SampleCode;

string mesText = @"


";

MesBuilder builder = new MesBuilder(mesText, new MesConfig());

// Default Build Pattern.
// if you want use var, using `using Mes.core;` and var replace to Mes.core.Mes 
var mesA = builder.Build();

// Custom Build Pattern.
builder.DoFlat_HashiraToComment();	//this method replace builder.RawText
var mesB = builder.Build(skipDoFlat:true);		//Skip default DoFlat methods.


```


## Tips

### .NETのバージョン

Mesのターゲットフレームワークは.NET 7です。

### PowerShellにおける拡張メソッド

PowerShellはC#の拡張メソッドをC#のように解釈しません。
拡張メソッドは静的クラスのメソッドとして読み込まれます。
