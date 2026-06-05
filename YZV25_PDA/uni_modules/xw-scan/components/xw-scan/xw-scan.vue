<!-- <template>
	<view>
		<view class="content"></view>
	</view>
</template>

<script>
	var main, receiver, filter;
	var codeQueryTag = false;
	export default {
		data() {
			return {
				scanCode: ''
			}
		},
		created() {
			this.initScan()
			this.startScan();
		},
		onHide() {
			this.stopScan();
		},
		destroyed() {
			this.stopScan();
		},
		methods: {
			initScan() {
				//  #ifdef APP
				console.log('initScan');
				let that = this;
				main = plus.android.runtimeMainActivity(); //获取activity
				//var context = plus.android.importClass('android.content.Context'); //上下文
				var IntentFilter = plus.android.importClass('android.content.IntentFilter');
				filter = new IntentFilter();
				//下面的addAction内改为自己的广播动作
				//filter.addAction("android.intent.action.SCANRESULT");	
				filter.addAction("com.service.scanner.data");	
				receiver = plus.android.implements('io.dcloud.feature.internal.reflect.BroadcastReceiver', {
					onReceive: (context, intent)=> {
						console.log('onReceive');
						plus.android.importClass(intent);
						//下面的getStringExtra内改为自己的广播标签--有误
						//let code = intent.getStringExtra("value");
					    let code = intent.getStringExtra("ScanCode");
						that.queryCode(code);							
					}
				});
				// #endif
			},
			startScan() {
				//  #ifdef APP
				console.log('startScan');
				main.registerReceiver(receiver, filter);
				// #endif
			},
			stopScan() {
				//  #ifdef APP
				console.log('stopScan');
				main.unregisterReceiver(receiver);
				// #endif
			},
			queryCode: function(code) {
				//  #ifdef APP
				console.log('queryCode');
				if (codeQueryTag) return false;
				codeQueryTag = true;
				setTimeout(function() {
					codeQueryTag = false;
				}, 150);
				var id = code
				uni.$emit('xwscan', {
					code: id
				})
				// #endif
			}
		}
	}
</script>

<style>

</style> -->

<template>
	<view>
		<view class="content"></view>
	</view>
</template>

<script> 
	var main, receiver, filter;
	var codeQueryTag = false;
	export default {
		data() {
			return {
				scanCode: ''
			}
		},
		created() {
			this.initScan()
			this.startScan();
		},
		onHide() {
			this.stopScan();
		},
		destroyed() {
			this.stopScan();
		},
		methods: {
			initScan() {
				//  #ifdef APP
				console.log('initScan');
				let that = this;
				main = plus.android.runtimeMainActivity(); //获取activity
				//var context = plus.android.importClass('android.content.Context'); //上下文
				var IntentFilter = plus.android.importClass('android.content.IntentFilter');
				filter = new IntentFilter();
				//下面的addAction内改为自己的广播动作
				filter.addAction("android.intent.action.SCANRESULT");	
				// filter.addAction("com.service.scanner.data");	
				receiver = plus.android.implements('io.dcloud.feature.internal.reflect.BroadcastReceiver', {
					onReceive: (context, intent)=> {
						console.log('onReceive');
						plus.android.importClass(intent);
						//下面的getStringExtra内改为自己的广播标签--有误
						
						let code = intent.getStringExtra("value");
					    // let code = intent.getStringExtra("ScanCode");
						that.queryCode(code);							
					}
				});
				// #endif
			},
			startScan() {
				//  #ifdef APP
				console.log('startScan');
				main.registerReceiver(receiver, filter);
				// #endif
			},
			stopScan() {
				//  #ifdef APP
				console.log('stopScan');
				main.unregisterReceiver(receiver);
				// #endif
			},
			queryCode: function(code) {
				//  #ifdef APP
				console.log('queryCode');
				if (codeQueryTag) return false;
				codeQueryTag = true;
				setTimeout(function() {
					codeQueryTag = false;
				}, 150);
				var id = code
				uni.$emit('xwscan', {
					code: id
				})
				// #endif
			}
		}
	}
</script>

<style>

</style>

