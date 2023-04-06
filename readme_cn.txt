Unity Demo

一、功能介绍：

	1.聊天室频道：
	-- 包含3个聊天室频道(World, Guild, Party)；
	-- 在聊天室频道可以发送聊天室消息；
	-- 可以通过点击聊天室消息右上角的图标添加好友或者是切换到好友单聊页面；
	
	2.好友列表：
	-- 点击某一个好友切换至单聊页面；
	
	3.单聊页面：
	-- 在收到好友申请时，可以接受或者拒绝；
	-- 可以和好友相互收发消息；
	
	4.支持emoji
	-- 发送文本消息时支持嵌入emoji表情符号；
	
	5.新消息提示
	-- 收到新消息时，如果该消息不属于当前频道，会通过红圈+数字进行提示，最大显示99+；
	
二、使用说明：

	1.zip包解压
	
	2.配置文件：
		
		配置文件在Mac平台下位于：Contents/Resources/Data/StreamingAssets
		配置文件在Windows平台下位于:unity_demo_Data\StreamingAssets
		
		缺省配置如下:	

		AppKey=easemob-demo#unitytest
		UseToken=false
		UserName=unity_demo
		Password=unity_demo
		Token=xxx
		World=205360469180417
		Guild=205360500637703
		Party=205360528949251
		
		缺省情况下使用的是用户名+密码登录，可通过UseToken改成（true）使用声网token登录，同时Token需要设置声网token；
		World，Guild，Party分别为3个在相应Appkey下的聊天室频道。如果Appkey进行变更，World，Guild，Party必须设置成该Appkey下
		的3个聊天室id。
		
	3.运行程序:
		Mac平台下，运行unity_demo即可
		Windows平台下，运行unityunity_demo.exe即可
		
	4.日志查看：
		Mac平台下，Unity输出log位于Console下的Player.log，sdk自身的log位于/Users/xxx/Library/Application Support/DefaultCompany/unity_demo/sdkdata/easemobLog  （xxx为用户名）
		Windows平台下，Unity输出log位于C:\Users\xxx\AppData\LocalLow\DefaultCompany\unity_demo 下（xxx为用户名），sdk自身的log也在同目录的sdkdata \easemobLog下