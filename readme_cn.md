# Unity Demo


## 功能介绍

### 1 聊天室频道
- 包含3个聊天室频道(World, Guild, Party)；
- 在聊天室频道可以发送聊天室消息；
- 可以通过点击聊天室消息右上角的图标添加好友或者是切换到好友单聊页面；
 
### 2 好友列表
- 点击某一个好友切换至单聊页面;

### 3 单聊页面
- 在收到好友申请时，可以接受或者拒绝；
- 可以和好友相互收发消息；

### 4 支持emoji
- 发送文本消息时支持嵌入emoji表情符号；

### 5 新消息提示
- 收到新消息时，如果该消息不属于当前频道，会通过红圈+数字进行提示，最大显示99+；


## Demo执行程序说明
- 下载zip包，并解压
    [windows版本](https://downloadsdk.easemob.com/downloads/SDK/Demo/WinExe.zip)
    [mac版本](https://downloadsdk.easemob.com/downloads/SDK/Demo/MacApp.zip)

- 配置文件：sdk.txt
	-   配置文件在Mac平台下位于：Contents/Resources/Data/StreamingAssets
	-   配置文件在Windows平台下位于:unity_demo_Data\StreamingAssets
		
		缺省配置如下:	

		- AppKey=easemob-demo#unitytest
		- UseToken=false
		- UserName=unity_demo
		- Password=unity_demo
		- Token=xxx
		- World=205360469180417
		- Guild=205360500637703
		- Party=205360528949251
		
		缺省情况下使用的是用户名+密码登录，可通过UseToken改成（true）使用声网token登录，同时Token需要设置声网token；
		World，Guild，Party分别为3个在相应Appkey下的聊天室频道。如果Appkey进行变更，World，Guild，Party必须设置成该Appkey下
		的3个聊天室id。
		
- 运行程序

    Mac平台下，运行unity_demo即可
	Windows平台下，运行unityunity_demo.exe即可
	
- 日志查看

    Mac平台下，Unity输出log位于Console下的Player.log，sdk自身的log位于/Users/xxx/Library/Application Support/DefaultCompany/unity_demo/sdkdata/easemobLog  （xxx为用户名）

    Windows平台下，Unity输出log位于C:\Users\xxx\AppData\LocalLow\DefaultCompany\unity_demo 下（xxx为用户名），sdk自身的log也在同目录的sdkdata\easemobLog下

## 二次开发说明
### 1 导入包
导入方式：在Unity Editor下，通过菜单Assets->Import Package选择需要导入的unitypackage；
- unity_demo包：所有界面元素及脚本的包, [下载地址](https://downloadsdk.easemob.com/downloads/SDK/Demo/unity_demo.unitypackage)
- AgoraChat Sdk包：AgoraChat Sdk, [下载地址](https://docs.agora.io/en/sdks?platform=unity)

### 2 包内容介绍
在导入unity_demo包和AgoraChat SDK包后，Assets目录里的相关内容如下：
- AgoraChat：Agora Chat SDK相关内容，包含c#代码以及Chat plugins；
- Resources：资源文件夹(注意名称不可更改)。其下有包括以下内容：
    - AppIcons：图标资源；
    - Avatars：用户头像资源；
    - Cursors：鼠标资源；
    - Emojis：Emoji表情资源；
    - Icons：图标资源；
    - Images：图片资源；
    - Others：其他资源，比如窗体边框等；
    - Scences：场景目录；

- Scripts：脚本目录；
    - ActionButton.cs: 对应canvas/BackGroundPanel/ActionButton，用来弹出其他功能按钮；
    - AddFriendButton.cs: 对应canvas/AddFriendButton按钮，用来添加好友；
    - ApplyActionButton.cs: 对应UIResources/ApplyActionButton（预设体），用来接受或者忽略好友请求；
    - ChatContainerPanel.cs: 对应canvas/MainPanel/ChatContainerPanel，单聊界面容器，管理单聊页面；
    - ChatSwitchButton.cs: 对应canvas/ChatSwitchButton，用来切换至单聊页面；
    - Context.cs: 上下文脚本，记录SDK是否初始化成功，当前显示的页面是哪一个等；
    - DateSplitButton.cs: 对应UIResources/DateSplitButton（预设体），用来添加当天的日期信息；
    - EmojiPanel.cs: 对应canvas/EmojiPanel，用来显示和隐藏EmojiPanel面板；
    - ExitButton.cs: 对应canvas/BackGroundPanel/ExitButton，用来退出App；
    - FriendItemButton.cs： 对应UIResources/FriendItemButton（预设体），用来在添加好友列表；
    - FriendsScrollView.cs：对应canvas/MainPanel/ContainerPanel/FriendsScrollView，用来管理好友列表；
    - HideButton.cs: 对应canvas/HideButton按钮，用来隐藏主窗体；
    - HintBubbleButton.cs：对应UIResources/HintBubbleButton（预设体），用来提示新消息数目；
    - InvokeButton.cs: 对应canvas/BackGroundPanel/InvokeButton，用来唤醒主窗体；
    - MainPanel.cs: 对应canvas/MainPanel，所有主体页面容器；
    - MessageActionImage.cs: 对应UIResources/MessageButton/ActionButton/BackGroundButton/MessageActionImage，用于唤醒添加好友按钮或者是切换单聊按钮；
    - MessageButton.cs: 对应UIResources/MessageButton（预设体），用于在界面上添加聊天消息；
    - MessageHintButton.cs: 对应UIResources/MessageHintButton（预设体）, 用于在界面上添加消息提示；
    - MyResources.cs: 资源脚本，用于加载Resources下的各种资源并提供使用接口；
    - ScrollView.cs: 对应UIResources/ScrollView（预设体）, 聊天消息和好友列表的容器；
    - Sdk.cs: SDK封装脚本，用于获取配置文件中信息，登录，加入聊天室，提供收发消息功能等，同时在收到回调或者消息时通知UIManager；
    - SendContainerButton.cs: 对应canvas/MainPanel/SendContainerButton，用于输入和发送消息以及唤醒Emoji面板；
    - SingleEmojiButton.cs: 对应UIResources/SingleEmojiButton（预设体），用来添加一个Emoji表情到发送消息的文本上；
    - TabButton.cs: 对应canvas/MainPanel/TabButton，用来控制控制主窗体上4个Tab按钮的切换和显示；
    - Tools.cs: 基础功能脚本，提供基础函数，如：物体对象的显示与隐藏，配置文件解析，目录获取，Emoji字符转换和unicode转换以及时间操作等；
    - UIManager.cs: 主要UI物体对象管理脚本，用于传递通知主要UI物体进行某种操作；
    
- StreamingAssets：配置文件目录，存放配置文件sdk.txt

- TextMeshPro：资源包，在引入可伸缩字体时，Unity自动引入；其中
    - TextMeshPro->Resources->TMP Settings->Default Sprite Asset用于设置文本上显示的Emoji资源；
    - TextMeshPro->Sprites下包含两个emojis资源文件，一个是json格式的emojis数据文件，一个是emojis的图片文件。用于生成Default Sprite Asset使用的Emoji资源；
    
- UIResources：预设体资源；双击每个预设体文件，可以看到里面物体的内容
    - ApplyActionButton： 接受或者忽略好友请求的预设体；
    - DateSplitButton：显示日志和分隔线的预设体；
    - FriendItemButton：好友列表项预设体；
    - FriendSplitButton：好友列表分隔线预设体；
    - HintBubbleButton：提示泡泡预设体；
    - MessageButton：聊天消息预设体;
    - MessageHintButton: 提示内容预设体；
    - ScrollView：聊天消息和好友列表的容器预设体；
    - SingleEmojiButton：单个Emoji表情预设体；
    - TabButton：频道切换键预设体；

### 3 unity_demo代码架构介绍
参考structure.png

### 4 可二次开发的一些点
unity_demo中的所有内容均可以修改，建议可二次开发的点有如下几个地方:

- 增加聊天室tab
    1. 添加tab: 从UIResources拖拽一个TabButton到Hierarchy的canvas/MainPanel/TabsButton下;
    2. 修改tab: 选中TabButton，修改其下的Text内容和Image内容即可;

- 删除聊天室tab
    在场景中选中需要删除的TabButton，点击删除即可

- 调整ScrollView中内容的显示
    1. 修改预设体: ScrollView中显示的内容主要包含两个:MessageButton和FriendItemButton; 在UIResources中选中MessageButton或FriendItemButton中的任何一个预设体，双击进入到修改页面，进行对应元素的变更即可;
    3. 修改对应脚本: 当进行界面元素变更后（例如，界面元素被删除或者修改了名字），对应的脚本也需要进行相应的变更;
    
- 在ScrollView里动态增加新的内容
    1. 制作预设体: 可以先在canvas下进行组件设计和组合，当风格满足需求时，将组合组件拖入到UIResources，生成预设体；
    2. 添加消息转发: 在ScrollView添加对新预设体的消息转发;
    3. 添加脚本: 给新预设体添加脚本文件，处理接收到的消息，然后触发界面操作；

- 修改Emoji内容
    1. 素材获取：下载或者制作Emoji素材；
    2. 生成Emoji数据和图片文件：使用TexturePackerGUI将所有的Emoji素材打包到两个文件中，一个json数据文件，一个png图片文件，TexturePackerGUI还可以对每一个Emoji的显示中心等进行调整；
    3. 生成Emoji资源文件：在Unity Editor下，通过菜单Windows->TextMeshPro->Sprite Importer下：将json文件拖拽到Sprite Data Source里，将png文件拖拽到Sprite Texture Atlas里。依次点击Create Sprite Asset和Save Sprite Asset生成Emoji资源文件，将生成的资源文件拷贝到TextMeshPro/Resources/Sprite Assets目录下；
    4. 设置Emoji资源为缺省资源：在Project目录里，打开TextMeshPro->Resources->TMP Settings，将刚刚生成的资源文件拖拽到Default Sprite Asset上即可；
    5. 调整Emoji的Unicode编码：如果生成的Emoji资源文件存在Unicode不对的情况，可以双击Emoji资源文件，然后在Inspector中，找到Sprite Character Table，打开调整里面的Unicode
    注意：需要点击一下Unicode下方的Edit Glyph，在打开的内容里面任意点击一个文本框即可，否则修改Unicode不成功

- 修改App图标
    1. 图标获取：下载或者制作App图标；
    2. 将图标加入项目：将图标拖拽到Resources/AppIcons目录下；
    3. 转换图标格式：选中图标，在Inspector的Texture Type中选择Sprite(2D and UI)类型，然后应用；
    4. 引用图标：打开菜单File->Build Settting->Player settings->Player，将图标拖拽如Default Icon中即可；

- 更改用户头像
    1. 头像获取：下载或者制作用户头像；
    2. 将头像加入到项目：将图标拖拽到Resources/Avatars目录下；
    3. 转换头像格式：选中图标，在Inspector的Texture Type中选择Sprite(2D and UI)类型，然后应用；
    4. 调整头像使用逻辑：调整Tools.cs中GetAvatarIndex和MyResources.cs的GetAvatarName逻辑，根据用户名获取头像名称；
