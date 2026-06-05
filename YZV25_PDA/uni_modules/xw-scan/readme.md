# xw-scan

## 功能及测试

- 基于uniapp开发，使用在PDA上面,对PDA扫描头部的监听,可以在PDA扫码后获取到对应的数据
- 测试设备是ZEBRA (Android 10),打包app，测试正常
- 该插件仅限于PDA扫描模式为广播模式
- 广播模式不设置无法进行扫描
- 可以进行RFID电子标签识别，条形码红外扫描识别

## 使用说明

- 插件导入在需要的页面直接使用，下面使用模板
	
### 使用方法
```
	<xw-scan></xw-scan>
```
```
	<xw-scan />
```
onUnload中先移除监听事件

onShow中先移除监听事件后监听事件

移除事件

```
uni.$off('xwscan')
```

监听事件

```
uni.$on('xwscan', (res)=> {
	console.log('扫码结果：', res.code);
	this.input = res.code
})
```
### 需要在PAD设置广播动作 广播标签
	广播动作："com.dwexample.ACTION"
	广播标签：com.motorolasolutions.emdk.datawedge.data_string
	
```
	广播动作："com.dwexample.ACTION"
	广播标签：com.motorolasolutions.emdk.datawedge.data_string

```

