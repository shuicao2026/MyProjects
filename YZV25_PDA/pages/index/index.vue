<template>
	<view class="scan-container" :class="['theme-' + currentTheme]">
		<!-- 扫描组件 -->
		<xw-scan></xw-scan>
		
		<!-- 自定义导航栏 -->
		<view class="custom-nav">
			<view class="nav-left" @click="goToHistory">
				<view class="nav-icon history-icon-small"></view>
				<text class="nav-text">历史</text>
			</view>
			<text class="nav-title">{{ jt || 'V25 工作站' }}</text>
			<view class="nav-right" @click="goToSetting">
				<text class="nav-text">设置</text>
				<view class="nav-icon setting-icon-small"></view>
			</view>
		</view>
		

		
		<!-- 扫码结果列表 -->
		<view class="scan-list">
			<view class="list-header">
				<view class="header-accent"></view>
				<text class="header-title">扫描记录</text>
				<text class="header-count">{{results.length}}/10</text>
			</view>
			
			<scroll-view class="list-scroll" scroll-y="true">
				<view 
					class="scan-item" 
					v-for="(item, index) in results" 
					:key="item.id"
					:class="{'current': index === 0, 'uploading': item.status === '上传中'}"
				>
					<!-- 顶部栏：序号 + 状态 + 操作 -->
					<view class="item-top-bar">
						<view class="item-badge" :class="getBadgeClass(index)">
							<text class="badge-number">{{index + 1}}</text>
						</view>
						<view class="status-group">
							<view class="status-tag" :style="{backgroundColor: item.color + '20', borderColor: item.color, color: item.color}">
								<text class="tag-text">{{item.status}}</text>
							</view>
							<view class="error-detail-btn" v-if="item.status === '上传失败' && item.errorInfo" @click="showErrorDetail(item.errorInfo)" title="查看错误详情">
								<view class="error-detail-icon"></view>
								<text class="error-detail-text">详情</text>
							</view>
						</view>
						<view class="action-group">
							<view class="action-icon" v-if="index === 0" @click="copyBarcode(item.code)" title="复制">
								<view class="icon-copy"></view>
							</view>
						</view>
					</view>
					
					<!-- 条码内容 - 无框显示 -->
					<view class="barcode-content">
						<text class="barcode-text" selectable>{{item.code}}</text>
					</view>
				</view>
				
				<!-- 空状态 -->
				<view class="empty-state" v-if="results.length === 0">
					<view class="empty-icon">
						<view class="scan-icon"></view>
					</view>
					<text class="empty-title">等待扫描</text>
					<text class="empty-desc">请使用扫码枪扫描条码</text>
				</view>
			</scroll-view>
		</view>
		
		<!-- 提示消息 -->
		<view class="toast-message" v-if="showToast" :class="{'toast-show': showToast}">
			<view class="toast-icon" :class="toastType"></view>
			<text class="toast-text">{{toastMessage}}</text>
		</view>
		
		<!-- 底部强制完成按钮 -->
		<view class="bottom-action">
			<view class="force-complete-btn" @click="showConfirmPwdModal">
				<text class="btn-text">强制条码出站</text>
			</view>
		</view>
		
		<!-- 密码验证弹窗 -->
		<view class="modal-overlay" v-if="showPwdModal" @click="closePwdModal">
			<view class="modal-content" @click.stop>
				<view class="modal-header">
					<text class="modal-title">强制条码出站</text>
				</view>
				<view class="modal-body">
					<view class="pwd-input-wrapper">
						<input 
							v-show="false"
							
							class="pwd-input" 
							type="text" 
							v-model="barcode" 
							placeholder="请输入条码,空则为该PDA最新扫的条码"
							placeholder-class="pwd-placeholder"
						/>
					</view>
					<view class="pwd-input-wrapper">
						<input 
							:focus='true'
							class="pwd-input" 
							type="password" 
							v-model="pwdInput" 
							placeholder="请输入密码"
							placeholder-class="pwd-placeholder"
						/>
					</view>
					<view class="face-selector" v-if="deviceType==='CMT'">
						<view 
							class="face-option" 
							:class="{'active': selectedFace === 'A'}"
							@click="selectedFace = 'A'"
						>
							<view class="face-radio" :class="{'checked': selectedFace === 'A'}"></view>
							<text class="face-text">A面</text>
						</view>
						<view 
							class="face-option" 
							:class="{'active': selectedFace === 'B'}"
							@click="selectedFace = 'B'"
						>
							<view class="face-radio" :class="{'checked': selectedFace === 'B'}"></view>
							<text class="face-text">B面</text>
						</view>
					</view>
				</view>
				<view class="modal-footer">
					<view class="modal-btn modal-btn-cancel" @click="closePwdModal">
						<text class="btn-label">取消</text>
					</view>
					<view class="modal-btn modal-btn-confirm" @click="confirmPwd">
						<text class="btn-label">确定</text>
					</view>
				</view>
			</view>
		</view>
	</view>
</template>

<script>
	export default {
		data() {
			return {
				results: [],
				timer: null,
				ip: '',
				id: '',
				jt: '',
				deviceType:'',
				port: '6060',
				apiPath: 'api/pda/scan',
				faceType: "A",
				scanLock: false,
				expandedItems: {},
				showToast: false,
				toastMessage: '',
				toastType: 'success',
				toastTimer: null,
				currentTheme: 'light',
				// 密码验证弹窗相关
				showPwdModal: false,
				barcode:'',
				pwdInput: '',
				selectedFace: 'A'
			}
		},
		onLoad() {
			this.loadConfig();
			this.loadThemeFromStorage();
		},
		onUnload() {
			console.log('onUnload');
			uni.$off('xwscan');
		},
		onHide() {
			console.log('onHide');
		},
		methods: {
			// 从本地存储加载主题
			loadThemeFromStorage() {
				const savedTheme = uni.getStorageSync('appTheme');
				if (savedTheme) {
					this.currentTheme = savedTheme;
				}
			},
			
			// 加载配置
			loadConfig() {
				this.$dbUtils.selectDataList('dataDB', 'mes', {id: 1}, 'id', 'desc').then(res => {
					if (res.length >= 1) {
						this.ip = res[0].ip;
						this.id = res[0].gx;
						this.jt = res[0].jt;
						this.deviceType=res[0].deviceType;
						this.port = res[0].port || '6060';
						this.apiPath = res[0].apiPath || 'api/pda/scan';
						uni.setNavigationBarTitle({title: res[0].jt});
					} else {
						this.ip = '192.168.60.36';
					}
				});
			},
			
			// 生成唯一ID
			generateId() {
				return Date.now() + '_' + Math.floor(Math.random() * 1000);
			},
			
			// 根据ID更新状态
			updateStatusById(id, status, color, errorInfo = null) {
				const targetIndex = this.results.findIndex(item => item.id === id);
				if (targetIndex !== -1) {
					this.results.splice(targetIndex, 1, {
						...this.results[targetIndex],
						status: status,
						color: color,
						errorInfo: errorInfo
					});
				}
			},
			
			// 显示错误详情
			showErrorDetail(errorInfo) {
				let detail = '';
				if (typeof errorInfo === 'object') {
					detail = JSON.stringify(errorInfo, null, 2);
				} else {
					detail = errorInfo;
				}
				uni.showModal({
					title: '错误详情',
					content: detail,
					showCancel: false,
					confirmText: '知道了',
					confirmColor: '#409eff'
				});
			},
			
			// 获取徽章样式类
			getBadgeClass(index) {
				if (index === 0) return 'badge-current';
				if (index === 1) return 'badge-second';
				if (index === 2) return 'badge-third';
				return 'badge-normal';
			},
			
			// 复制条码
			copyBarcode(code) {
				uni.setClipboardData({
					data: code,
					success: () => {
						this.showToastMessage('条码已复制', 'success');
					}
				});
			},
			
				// 显示提示
			showToastMessage(message, type = 'success') {
				this.toastMessage = message;
				this.toastType = type;
				this.showToast = true;
				
				if (this.toastTimer) {
					clearTimeout(this.toastTimer);
				}
				
				this.toastTimer = setTimeout(() => {
					this.showToast = false;
				}, 2000);
			},
			
			// 跳转到设置页面
			goToSetting() {
				uni.navigateTo({
					url: 'index3',
					fail: (err) => {
						console.log('跳转失败:', err);
						uni.showToast({
							title: '页面跳转失败',
							icon: 'none'
						});
					}
				});
			},
			
			// 跳转到历史记录页面
			goToHistory() {
				uni.navigateTo({
					url: 'index2',
					fail: (err) => {
						console.log('跳转失败:', err);
						uni.showToast({
							title: '页面跳转失败',
							icon: 'none'
						});
					}
				});
			},
			
			// 显示密码验证弹窗
			showConfirmPwdModal() {
				this.showPwdModal = true;
				this.pwdInput = '';
				this.selectedFace = 'A';
			},
			
			// 关闭密码验证弹窗
			closePwdModal() {
				this.showPwdModal = false;
				this.pwdInput = '';
			},
			
			// 确认密码
			confirmPwd() {
				if (!this.pwdInput) {
					this.showToastMessage('请输入密码', 'error');
					return;
				}
				
				uni.showLoading({title: '验证中...', mask: true});
				
				const fullApiUrl = 'http://' + this.ip + ':' + this.port + '/api/pda/force';
				const requestData = {
					id: this.id,
					code: this.barcode,
					pwd:this.pwdInput,
					
					face: this.selectedFace
				};
				
				console.log('========== 密码验证信息 ==========');
				console.log('路由地址:', fullApiUrl);
				console.log('验证数据:', JSON.stringify(requestData));
				console.log('==================================');
				
				uni.request({
					url: fullApiUrl,
					method: 'POST',
					data: JSON.stringify(requestData),
					header: {'content-type': 'application/json'},
					success: (res) => {
						console.log("密码验证返回：", res.data);
						if (res.data) {
							// let msg = JSON.parse(res.data);
							console.log("res.data:",res.data)
							let msg =this.getJson(res.data);// JSON.parse(res2.data);
							console.log(msg)
							if (msg.code == 0) {
								this.showToastMessage(msg.msg || '操作成功', 'success');
								this.closePwdModal();
							} else {
								this.showToastMessage(msg.msg || '密码错误', 'error');
							}
						} else {
							this.showToastMessage('服务器返回空数据', 'error');
						}
					},
					fail: (err) => {
						console.log("密码验证失败：", err);
						this.showToastMessage('服务器连接失败', 'error');
					},
					complete: () => {
						uni.hideLoading();
					}
				});
			},
		
			getJson(obj){
				
				  if (typeof str !== 'string') return obj;
				  try {
				    const obj = JSON.parse(str);
				    // 解析成功且结果是对象或数组（JSON 合法）
				    return  obj//typeof obj === 'object' && obj !== null;
				  } catch (e) {
				    return obj;
				  }
			}
		
		},
		onShow() {
			this.loadConfig();
			
			uni.$off('xwscan');
			
			uni.$on('xwscan', (res) => {
				if(this.showPwdModal){
					return
				}
				
				const nowcode = res.code;
				
				// 全局锁检查
				if (this.scanLock) {
					const shakeItem = {
						id: this.generateId(),
						code: nowcode,
						status: '抖动',
						color: '#909399'
					};
					if (this.results.length >= 10) {
						this.results.pop();
					}
					this.results.unshift(shakeItem);
					console.log('锁定中，本次扫码标记为抖动：', nowcode);
					return;
				}
				
				// 生成当前扫码项
				const currentId = this.generateId();
				const currentItem = {
					id: currentId,
					code: nowcode,
					status: '上传中',
					color: '#409EFF'
				};
				
				// 上锁并添加到列表
				this.scanLock = true;
				if (this.results.length >= 10) {
					this.results.pop();
				}
				this.results.unshift(currentItem);
				uni.showLoading({title: '处理中...', mask: true});
				
				// 扫码后立即保存到数据库（无论上传结果如何）
				this.$dbUtils.addTabItem('dataDB', 'data', {scancode: nowcode}).then(res => {
					console.log('扫码数据已保存到数据库:', nowcode);
				}).catch(err => {
					console.log('保存到数据库失败:', err);
				});
				
				let timeoutTimer = null;
				
				// 超时处理
				timeoutTimer = setTimeout(() => {
					this.updateStatusById(currentId, '响应超时', '#E6A23C');
					this.scanLock = false;
					uni.hideLoading();
					this.showToastMessage('服务器响应超时', 'error');
				}, 5000);
				
				// 请求参数
				res.id = this.id;
				res.face = this.faceType;
				const fullApiUrl = 'http://' + this.ip + ':' + this.port +'/' + this.apiPath;
				
				// 打印上传信息
				console.log('========== 扫码上传信息 ==========');
				console.log('路由地址:', fullApiUrl);
				console.log('扫码内容:', nowcode);
				console.log('==================================');
				
				uni.request({
					url: fullApiUrl,
					method: 'POST',
					data: JSON.stringify(res),
					header: {'content-type': 'application/json'},
					success: (res2) => {
						console.log("服务器返回：",res2.data);
						if (res2.data) {
							let msg =this.getJson(res2.data);// JSON.parse(res2.data);
							console.log("服务器返回msg：", currentId,msg.code);
							if (msg.code == 0) {
								this.updateStatusById(currentId, '上传成功', '#67C23A');
								this.showToastMessage('上传成功', 'success');
							} else {
								this.updateStatusById(currentId, '上传失败', '#F56C6C', {
									code: msg.code,
									message: msg.msg || '上传失败'
								});
								this.showToastMessage(msg.msg || '上传失败', 'error');
							}
						} else {
							this.updateStatusById(currentId, '上传失败', '#F56C6C', {
								message: '服务器返回空数据'
							});
						}
					},
					fail: (res3) => {
						console.log("请求失败：", res3.errMsg);
						this.updateStatusById(currentId, '上传失败', '#F56C6C', {
							message: '服务器连接失败',
							errMsg: res3.errMsg,
							statusCode: res3.statusCode
						});
						this.showToastMessage('服务器连接失败', 'error');
					},
					complete: () => {
						clearTimeout(timeoutTimer);
						this.scanLock = false;
						uni.hideLoading();
					}
				});
			});
		}
	}
</script>

<style>
	/* ==================== 浅色主题 (默认) ==================== */
	.theme-light {
		--bg-primary: #f5f7fa;
		--bg-secondary: #ffffff;
		--bg-card: #ffffff;
		--border-color: #e4e7ed;
		--text-primary: #303133;
		--text-secondary: #909399;
		--accent-color: #409eff;
		--accent-secondary: #67c23a;
		--accent-danger: #f56c6c;
		--accent-warning: #e6a23c;
		--shadow: rgba(0, 0, 0, 0.1);
	}

	/* ==================== 深色主题 ==================== */
	.theme-dark {
		--bg-primary: #1a1a2e;
		--bg-secondary: #16213e;
		--bg-card: #0f3460;
		--border-color: #1a1a2e;
		--text-primary: #eaeaea;
		--text-secondary: #a0a0a0;
		--accent-color: #e94560;
		--accent-secondary: #0f3460;
		--accent-danger: #ff6b6b;
		--accent-warning: #feca57;
		--shadow: rgba(0, 0, 0, 0.3);
	}

	/* ==================== 工业橙主题 ==================== */
	.theme-orange {
		--bg-primary: #faf8f5;
		--bg-secondary: #fff9f0;
		--bg-card: #ffffff;
		--border-color: #ffd8b8;
		--text-primary: #2c1810;
		--text-secondary: #8b6914;
		--accent-color: #ff6b35;
		--accent-secondary: #ff8c42;
		--accent-danger: #e74c3c;
		--accent-warning: #f39c12;
		--shadow: rgba(255, 107, 53, 0.15);
	}

	/* ==================== 基础样式 ==================== */
	.scan-container {
		min-height: 100vh;
		background: var(--bg-primary);
		padding: 20rpx;
		box-sizing: border-box;
		transition: all 0.3s ease;
		position: relative;
		padding-top: 160rpx;
	}

	/* 自定义导航栏 */
	.custom-nav {
		position: fixed;
		top: 40rpx;
		left: 0;
		right: 0;
		height: 90rpx;
		background: var(--bg-card);
		border-bottom: 2rpx solid var(--border-color);
		display: flex;
		justify-content: space-between;
		align-items: center;
		z-index: 99;
		box-shadow: 0 4rpx 16rpx var(--shadow);
		padding: 0 20rpx;
	}

	.nav-left, .nav-right {
		display: flex;
		align-items: center;
		padding: 10rpx 16rpx;
		border-radius: 8rpx;
		transition: all 0.3s ease;
	}

	.nav-left:active, .nav-right:active {
		background: var(--bg-secondary);
	}

	.nav-icon {
		width: 28rpx;
		height: 28rpx;
	}

	.history-icon-small {
		border: 3rpx solid var(--text-secondary);
		border-radius: 4rpx;
		position: relative;
		margin-right: 8rpx;
	}

	.history-icon-small::before {
		content: '';
		position: absolute;
		top: 5rpx;
		left: 50%;
		transform: translateX(-50%);
		width: 14rpx;
		height: 2rpx;
		background: var(--text-secondary);
		box-shadow: 0 5rpx 0 var(--text-secondary), 0 10rpx 0 var(--text-secondary);
	}

	.setting-icon-small {
		border: 3rpx solid var(--text-secondary);
		border-radius: 50%;
		position: relative;
		margin-left: 8rpx;
	}

	.setting-icon-small::before {
		content: '';
		position: absolute;
		top: 50%;
		left: 50%;
		transform: translate(-50%, -50%);
		width: 10rpx;
		height: 10rpx;
		background: var(--text-secondary);
		border-radius: 50%;
	}

	.nav-text {
		font-size: 24rpx;
		color: var(--text-secondary);
	}

	.nav-title {
		font-size: 34rpx;
		color: var(--text-primary);
		font-weight: bold;
		flex: 1;
		text-align: center;
	}

	/* 侧边按钮通用样式 */
	.side-btn {
		position: fixed;
		left: 0;
		width: 100rpx;
		background: var(--bg-card);
		border: 2rpx solid var(--border-color);
		border-left: none;
		border-radius: 0 12rpx 12rpx 0;
		display: flex;
		justify-content: center;
		align-items: center;
		z-index: 100;
		box-shadow: 4rpx 4rpx 16rpx var(--shadow);
		padding: 12rpx 8rpx;
	}

	.side-btn:active {
		background: var(--bg-secondary);
	}

	.btn-content {
		display: flex;
		flex-direction: column;
		align-items: center;
		justify-content: center;
	}

	.btn-label {
		font-size: 20rpx;
		color: var(--text-secondary);
		margin-top: 6rpx;
		font-weight: 500;
	}

	/* 扫码列表 */
	.scan-list {
		background: var(--bg-card);
		border: 2rpx solid var(--border-color);
		border-radius: 16rpx;
		overflow: hidden;
		box-shadow: 0 8rpx 32rpx var(--shadow);
		margin-top: 20rpx;
		padding: 20rpx;
		box-sizing: border-box;
	}

.list-header {
	display: flex;
	align-items: center;
	/* 修改：减少左右内边距，因为父容器已有 padding */
	padding: 24rpx 10rpx;
	background: var(--bg-secondary);
	border-bottom: 2rpx solid var(--border-color);
}

	.header-accent {
		width: 8rpx;
		height: 36rpx;
		background: linear-gradient(180deg, var(--accent-color), var(--accent-secondary));
		border-radius: 4rpx;
		margin-right: 16rpx;
	}

	.header-title {
		flex: 1;
		font-size: 30rpx;
		color: var(--text-primary);
		font-weight: bold;
	}

	.header-count {
		font-size: 26rpx;
		color: var(--text-secondary);
		background: var(--bg-primary);
		padding: 8rpx 20rpx;
		border-radius: 8rpx;
		border: 2rpx solid var(--border-color);
	}

.list-scroll {
	/* 修改：减去更多高度（头部高度 + 上下 padding） */
	max-height: calc(100vh - 380rpx);
	/* padding: 20rpx;  ← 删除这行 */
}


	/* 扫描项 */
.scan-item {
	padding: 24rpx;
	margin-bottom: 20rpx;
	background: var(--bg-secondary);
	border: 2rpx solid var(--border-color);
	border-radius: 16rpx;
	transition: all 0.3s ease;
	position: relative;
	/* 新增 */
	box-sizing: border-box;
}

	.scan-item:active {
		transform: scale(0.98);
	}

	.scan-item.current {
		border-color: var(--accent-color);
		/* 新增：彩色左边框替代伪元素 */
		border-left: 6rpx solid var(--accent-color);
		/* 新增：补偿左边框占用的空间，保持内容对齐 */
		padding-left: 20rpx;
	}

	

	.scan-item.uploading {
		border-color: var(--accent-color);
		animation: borderPulse 1.5s infinite;
	}

	@keyframes borderPulse {
		0%, 100% { border-color: var(--accent-color); box-shadow: 0 0 10rpx var(--shadow); }
		50% { border-color: var(--accent-secondary); box-shadow: 0 0 20rpx var(--shadow); }
	}

	/* 顶部栏 */
	.item-top-bar {
		display: flex;
		justify-content: space-between;
		align-items: center;
		margin-bottom: 16rpx;
	}

	/* 序号徽章 */
	.item-badge {
		width: 48rpx;
		height: 48rpx;
		border-radius: 10rpx;
		display: flex;
		justify-content: center;
		align-items: center;
		flex-shrink: 0;
	}

	.badge-current {
		background: linear-gradient(145deg, var(--accent-color), var(--accent-secondary));
		box-shadow: 0 4rpx 12rpx var(--shadow);
	}

	.badge-second {
		background: linear-gradient(145deg, #c0c4cc, #909399);
	}

	.badge-third {
		background: linear-gradient(145deg, #d4a574, #b8956a);
	}

	.badge-normal {
		background: var(--bg-primary);
		border: 2rpx solid var(--border-color);
	}

	.badge-number {
		font-size: 28rpx;
		color: #ffffff;
		font-weight: bold;
	}

	.badge-normal .badge-number {
		color: var(--text-secondary);
	}

	/* 状态标签 */
	.status-tag {
		padding: 6rpx 16rpx;
		border-radius: 6rpx;
		border: 2rpx solid;
		font-size: 22rpx;
		font-weight: 600;
		flex-shrink: 0;
		margin: 0 16rpx;
	}

	/* 状态组 */
	.status-group {
		display: flex;
		align-items: center;
		gap: 12rpx;
		flex: 1;
		margin: 0 16rpx;
	}

	/* 操作按钮组 */
	.action-group {
		display: flex;
		gap: 12rpx;
	}

	/* 操作按钮 */
	.action-icon {
		width: 48rpx;
		height: 48rpx;
		background: var(--bg-primary);
		border: 2rpx solid var(--border-color);
		border-radius: 10rpx;
		display: flex;
		justify-content: center;
		align-items: center;
		transition: all 0.3s ease;
		flex-shrink: 0;
	}

	.action-icon:active {
		background: var(--bg-secondary);
		transform: scale(0.95);
	}

	/* 错误详情按钮 */
	.error-detail-btn {
		display: flex;
		align-items: center;
		gap: 6rpx;
		padding: 6rpx 16rpx;
		background: rgba(245, 108, 108, 0.1);
		border: 2rpx solid #F56C6C;
		border-radius: 8rpx;
		transition: all 0.3s ease;
		flex-shrink: 0;
	}

	.error-detail-btn:active {
		background: rgba(245, 108, 108, 0.2);
		transform: scale(0.95);
	}

	.error-detail-icon {
		width: 20rpx;
		height: 20rpx;
		border: 2rpx solid #F56C6C;
		border-radius: 50%;
		position: relative;
	}

	.error-detail-icon::after {
		content: 'i';
		position: absolute;
		top: -4rpx;
		left: 4rpx;
		font-size: 14rpx;
		color: #F56C6C;
		font-weight: bold;
	}

	.error-detail-text {
		font-size: 22rpx;
		color: #F56C6C;
		font-weight: 500;
	}

	.icon-copy {
		width: 24rpx;
		height: 28rpx;
		border: 2rpx solid var(--text-secondary);
		border-radius: 3rpx;
		position: relative;
	}

	.icon-copy::before {
		content: '';
		position: absolute;
		top: -6rpx;
		right: -6rpx;
		width: 18rpx;
		height: 22rpx;
		border: 2rpx solid var(--text-secondary);
		border-radius: 3rpx;
		background: var(--bg-secondary);
	}

	/* 条码内容 - 无框显示 */
	.barcode-content {
		width: 100%;
		padding: 12rpx 0;
	}

	.barcode-text {
		font-size: 30rpx;
		color: var(--text-primary);
		font-family: 'Courier New', monospace;
		font-weight: 600;
		letter-spacing: 1rpx;
		word-break: break-all;
		line-height: 1.6;
	}

	/* 空状态 */
	.empty-state {
		display: flex;
		flex-direction: column;
		align-items: center;
		justify-content: center;
		padding: 100rpx 40rpx;
	}

	.empty-icon {
		width: 120rpx;
		height: 120rpx;
		background: var(--bg-secondary);
		border: 4rpx solid var(--border-color);
		border-radius: 50%;
		display: flex;
		justify-content: center;
		align-items: center;
		margin-bottom: 30rpx;
	}

	.scan-icon {
		width: 60rpx;
		height: 60rpx;
		border: 4rpx solid var(--text-secondary);
		border-radius: 8rpx;
		position: relative;
	}

	.scan-icon::after {
		content: '';
		position: absolute;
		top: 50%;
		left: -10rpx;
		width: calc(100% + 20rpx);
		height: 4rpx;
		background: var(--accent-color);
		animation: scanLine 2s infinite;
	}

	@keyframes scanLine {
		0%, 100% { transform: translateY(-20rpx); opacity: 1; }
		50% { transform: translateY(20rpx); opacity: 0.5; }
	}

	.empty-title {
		font-size: 32rpx;
		color: var(--text-primary);
		font-weight: bold;
		margin-bottom: 12rpx;
	}

	.empty-desc {
		font-size: 26rpx;
		color: var(--text-secondary);
	}

	/* 提示消息 */
	.toast-message {
		position: fixed;
		top: 50%;
		left: 50%;
		transform: translate(-50%, -50%) scale(0.8);
		background: var(--bg-card);
		border: 2rpx solid var(--border-color);
		border-radius: 16rpx;
		padding: 40rpx 60rpx;
		display: flex;
		flex-direction: column;
		align-items: center;
		opacity: 0;
		pointer-events: none;
		transition: all 0.3s ease;
		z-index: 9999;
		box-shadow: 0 8rpx 32rpx var(--shadow);
	}

	.toast-message.toast-show {
		transform: translate(-50%, -50%) scale(1);
		opacity: 1;
	}

	.toast-icon {
		width: 60rpx;
		height: 60rpx;
		border-radius: 50%;
		margin-bottom: 20rpx;
		position: relative;
	}

	.toast-icon.success {
		background: var(--accent-secondary);
	}

	.toast-icon.success::after {
		content: '';
		position: absolute;
		top: 50%;
		left: 50%;
		transform: translate(-50%, -60%) rotate(45deg);
		width: 16rpx;
		height: 28rpx;
		border: 6rpx solid #ffffff;
		border-left: none;
		border-top: none;
	}

	.toast-icon.error {
		background: var(--accent-danger);
	}

	.toast-icon.error::before,
	.toast-icon.error::after {
		content: '';
		position: absolute;
		top: 50%;
		left: 50%;
		width: 32rpx;
		height: 6rpx;
		background: #ffffff;
	}

	.toast-icon.error::before {
		transform: translate(-50%, -50%) rotate(45deg);
	}

	.toast-icon.error::after {
		transform: translate(-50%, -50%) rotate(-45deg);
	}

	.toast-text {
		font-size: 28rpx;
		color: var(--text-primary);
		font-weight: 500;
	}
	
	/* ==================== 底部操作区域 ==================== */
	.bottom-action {
		position: fixed;
		bottom: 30rpx;
		left: 30rpx;
		right: 30rpx;
		z-index: 98;
	}
	
	.force-complete-btn {
		background: linear-gradient(145deg, #409eff, #3088e8);
		border-radius: 16rpx;
		padding: 30rpx;
		text-align: center;
		box-shadow: 0 8rpx 24rpx rgba(64, 158, 255, 0.4);
		transition: all 0.3s ease;
		border: 2rpx solid #409eff;
	}
	
	.force-complete-btn:active {
		transform: translateY(4rpx);
		box-shadow: 0 4rpx 12rpx rgba(64, 158, 255, 0.3);
		background: linear-gradient(145deg, #3088e8, #409eff);
	}
	
	.force-complete-btn .btn-text {
		font-size: 32rpx;
		color: #ffffff;
		font-weight: bold;
		letter-spacing: 2rpx;
	}
	
	/* ==================== 弹窗样式 ==================== */
	.modal-overlay {
		position: fixed;
		top: 0;
		left: 0;
		right: 0;
		bottom: 0;
		background: rgba(0, 0, 0, 0.6);
		display: flex;
		justify-content: center;
		align-items: center;
		z-index: 9998;
		animation: fadeIn 0.3s ease;
	}
	
	@keyframes fadeIn {
		from { opacity: 0; }
		to { opacity: 1; }
	}
	
	.modal-content {
		width: 600rpx;
		background: #ffffff;
		border-radius: 20rpx;
		overflow: hidden;
		box-shadow: 0 12rpx 48rpx rgba(0, 0, 0, 0.3);
		animation: slideUp 0.3s ease;
	}
	
	@keyframes slideUp {
		from { 
			opacity: 0;
			transform: translateY(40rpx);
		}
		to { 
			opacity: 1;
			transform: translateY(0);
		}
	}
	
	.modal-header {
		padding: 40rpx 30rpx 20rpx;
		text-align: center;
		border-bottom: 2rpx solid #f0f0f0;
	}
	
	.modal-title {
		font-size: 34rpx;
		color: #303133;
		font-weight: bold;
	}
	
	.modal-body {
		padding: 40rpx 30rpx;
	}
	
	.pwd-input-wrapper {
		margin-bottom: 40rpx;
	}
	
	.pwd-input {
		width: 100%;
		height: 88rpx;
		border: 2rpx solid #e4e7ed;
		border-radius: 12rpx;
		padding: 0 24rpx;
		font-size: 32rpx;
		background: #fafafa;
		box-sizing: border-box;
	}
	
	.pwd-input:focus {
		border-color: #409eff;
		background: #ffffff;
	}
	
	.pwd-placeholder {
		color: #c0c4cc;
	}
	
	.face-selector {
		display: flex;
		justify-content: center;
		gap: 80rpx;
	}
	
	.face-option {
		display: flex;
		align-items: center;
		gap: 16rpx;
		padding: 16rpx 32rpx;
		border-radius: 12rpx;
		transition: all 0.3s ease;
	}
	
	.face-option.active {
		background: rgba(64, 158, 255, 0.1);
	}
	
	.face-option:active {
		transform: scale(0.95);
	}
	
	.face-radio {
		width: 36rpx;
		height: 36rpx;
		border: 4rpx solid #d9d9d9;
		border-radius: 50%;
		position: relative;
		transition: all 0.3s ease;
	}
	
	.face-radio.checked {
		border-color: #409eff;
	}
	
	.face-radio.checked::after {
		content: '';
		position: absolute;
		top: 50%;
		left: 50%;
		transform: translate(-50%, -50%);
		width: 20rpx;
		height: 20rpx;
		background: #409eff;
		border-radius: 50%;
	}
	
	.face-text {
		font-size: 30rpx;
		color: #606266;
		font-weight: 500;
	}
	
	.face-option.active .face-text {
		color: #409eff;
		font-weight: bold;
	}
	
	.modal-footer {
		display: flex;
		border-top: 2rpx solid #f0f0f0;
	}
	
	.modal-btn {
		flex: 1;
		height: 96rpx;
		display: flex;
		justify-content: center;
		align-items: center;
		transition: all 0.3s ease;
	}
	
	.modal-btn:active {
		background: #f5f7fa;
	}
	
	.modal-btn-cancel {
		border-right: 2rpx solid #f0f0f0;
	}
	
	.modal-btn-cancel .btn-label {
		font-size: 32rpx;
		color: #606266;
		font-weight: 500;
	}
	
	.modal-btn-confirm {
		background: linear-gradient(145deg, #409eff, #3088e8);
	}
	
	.modal-btn-confirm:active {
		background: linear-gradient(145deg, #3088e8, #409eff);
	}
	
	.modal-btn-confirm .btn-label {
		font-size: 32rpx;
		color: #ffffff;
		font-weight: bold;
	}
</style>
