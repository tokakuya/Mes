# �iWIP�jMes : light markup language for Japanese Scenario.

Mes�͓��{��V�i���I�̋L�q�ɓ��������y�ʋL�q����ł��B  
���݁A�J���r���ł��B  

�ڍׂ͌���HP���Q�Ƃ��Ă��������B

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

MesBuilder�N���X���g����Mes�e�L�X�g���r���h�iparse�j����p�^�[���������ł��B

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

### .NET�̃o�[�W����

Mes�̃^�[�Q�b�g�t���[�����[�N��.NET 7�ł��B

### PowerShell�ɂ�����g�����\�b�h

PowerShell��C#�̊g�����\�b�h��C#�̂悤�ɉ��߂��܂���B
�g�����\�b�h�͐ÓI�N���X�̃��\�b�h�Ƃ��ēǂݍ��܂�܂��B
