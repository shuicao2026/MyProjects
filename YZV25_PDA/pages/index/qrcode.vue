<template>
	<view class="industrial-container">
		<!-- 顶部标题栏 -->
		<view class="header">
			<view class="header-line"></view>
			<text class="header-title">二维码展示</text>
			<view class="header-line"></view>
		</view>
		
		<!-- 机台信息卡片 -->
		<view class="info-card">
			<view class="info-row">
				<text class="info-label">机台号</text>
				<text class="info-value">{{id || '--'}}</text>
			</view>
			<view class="info-row">
				<text class="info-label">机台名称</text>
				<text class="info-value">{{jt || '--'}}</text>
			</view>
			<view class="divider"></view>
			<view class="info-row">
				<text class="info-label">当前状态</text>
				<view class="status-indicator" :class="{'status-active': isOnline}">
					<view class="status-dot"></view>
					<text class="status-text">{{isOnline ? '在线' : '离线'}}</text>
				</view>
			</view>
		</view>
		
		<!-- 二维码展示区域 -->
		<view class="qrcode-section">
			<view class="section-header">
				<view class="section-icon"></view>
				<text class="section-title">设备二维码</text>
				<view class="section-line"></view>
			</view>
			
			<!-- 二维码容器 -->
			<view class="qrcode-wrapper">
				<view class="qrcode-border">
					<view class="corner corner-tl"></view>
					<view class="corner corner-tr"></view>
					<view class="corner corner-bl"></view>
					<view class="corner corner-br"></view>
					
					<!-- 二维码滚动容器 -->
					<scroll-view 
						class="qrcode-scroll" 
						scroll-x="true" 
						scroll-y="true"
						:scroll-left="scrollLeft"
						:scroll-top="scrollTop"
						@scroll="onScroll"
					>
						<view class="qrcode-content" :style="{transform: `scale(${scale})`}">
							<image 
								v-if="qrCodeUrl" 
								:src="qrCodeUrl" 
								class="qrcode-image" 
								mode="aspectFit"
								@load="onImageLoad"
								@error="onImageError"
							/>
							<view v-else class="qrcode-placeholder">
								<view class="loading-spinner"></view>
								<text class="placeholder-text">生成中...</text>
							</view>
						</view>
					</scroll-view>
					
					<!-- 缩放控制 -->
					<view class="zoom-controls">
						<view class="zoom-btn" @click="zoomOut" :class="{'disabled': scale <= 0.5}">
							<text class="zoom-icon">−</text>
						</view>
						<text class="zoom-level">{{Math.round(scale * 100)}}%</text>
						<view class="zoom-btn" @click="zoomIn" :class="{'disabled': scale >= 3}">
							<text class="zoom-icon">+</text>
						</view>
					</view>
				</view>
			</view>
			
			<!-- 二维码信息 -->
			<view class="qrcode-info">
				<text class="qrcode-label">二维码内容</text>
				<text class="qrcode-value" selectable>{{qrCodeContent || '暂无内容'}}</text>
			</view>
		</view>
		
		<!-- 操作按钮区域 -->
		<view class="action-section">
			<view class="action-row">
				<view class="action-btn btn-primary" @click="refreshQrCode" :class="{'btn-loading': isRefreshing}">
					<view class="btn-icon">
						<view class="refresh-icon" :class="{'rotating': isRefreshing}"></view>
					</view>
					<text class="btn-text">刷新二维码</text>
				</view>
				<view class="action-btn btn-secondary" @click="saveQrCode">
					<view class="btn-icon">
						<view class="save-icon"></view>
					</view>
					<text class="btn-text">保存图片</text>
				</view>
			</view>
			<view class="action-row">
				<view class="action-btn btn-tertiary" @click="shareQrCode">
					<view class="btn-icon">
						<view class="share-icon"></view>
					</view>
					<text class="btn-text">分享</text>
				</view>
				<view class="action-btn btn-tertiary" @click="copyContent">
					<view class="btn-icon">
						<view class="copy-icon"></view>
					</view>
					<text class="btn-text">复制内容</text>
				</view>
			</view>
		</view>
		
		<!-- 底部信息 -->
		<view class="footer">
			<view class="footer-line"></view>
			<text class="footer-text">工业扫码系统 V1.0</text>
			<view class="footer-line"></view>
		</view>
		
		<!-- 提示消息 -->
		<view class="toast-message" v-if="showToast" :class="{'toast-show': showToast}">
			<view class="toast-icon" :class="toastType"></view>
			<text class="toast-text">{{toastMessage}}</text>
		</view>
	</view>
</template>

<script>
	export default {
		data() {
			return {
				// 机台信息
				id: '',
				jt: '',
				ip: '',
				port: '6060',
				apiPath: 'api/pda/scan',
				isOnline: true,
				
				// 二维码
				qrCodeUrl: '',
				qrCodeContent: '',
				
				// 缩放控制
				scale: 1,
				minScale: 0.5,
				maxScale: 3,
				scaleStep: 0.25,
				scrollLeft: 0,
				scrollTop: 0,
				
				// 状态
				isRefreshing: false,
				imageLoaded: false,
				
				// 提示
				showToast: false,
				toastMessage: '',
				toastType: 'success',
				toastTimer: null
			}
		},
		onLoad() {
			this.loadConfig();
			this.generateQrCode();
		},
		onShow() {
			this.loadConfig();
		},
		methods: {
			// 加载配置
			loadConfig() {
				this.$dbUtils.selectDataList('dataDB', 'mes', {id: 1}, 'id', 'desc').then(res => {
					if (res.length >= 1) {
						this.id = res[0].gx;
						this.jt = res[0].jt;
						this.ip = res[0].ip;
						this.port = res[0].port || '6060';
						this.apiPath = res[0].apiPath || 'api/pda/scan';
						uni.setNavigationBarTitle({title: res[0].jt + ' - 二维码'});
					}
				});
			},
			
			// 生成二维码
			generateQrCode() {
				this.isRefreshing = true;
				this.imageLoaded = false;
				
				// 构建二维码内容
				const content = JSON.stringify({
					id: this.id,
					jt: this.jt,
					ip: this.ip,
					timestamp: Date.now()
				});
				this.qrCodeContent = content;
				
				// 使用二维码生成API
				const qrApi = `https://api.qrserver.com/v1/create-qr-code/?size=400x400&data=${encodeURIComponent(content)}`;
				this.qrCodeUrl = qrApi;
				
				setTimeout(() => {
					this.isRefreshing = false;
				}, 500);
			},
			
			// 刷新二维码
			refreshQrCode() {
				if (this.isRefreshing) return;
				this.generateQrCode();
				this.showToastMessage('二维码已刷新', 'success');
			},
			
			// 保存二维码
			saveQrCode() {
				if (!this.qrCodeUrl) {
					this.showToastMessage('二维码未生成', 'error');
					return;
				}
				
				uni.downloadFile({
					url: this.qrCodeUrl,
					success: (res) => {
						if (res.statusCode === 200) {
							uni.saveImageToPhotosAlbum({
								filePath: res.tempFilePath,
								success: () => {
									this.showToastMessage('保存成功', 'success');
								},
								fail: () => {
									this.showToastMessage('保存失败', 'error');
								}
							});
						}
					},
					fail: () => {
						this.showToastMessage('下载失败', 'error');
					}
				});
			},
			
			// 分享二维码
			shareQrCode() {
				uni.share({
					title: '设备二维码',
					path: '/pages/index/qrcode',
					success: () => {
						this.showToastMessage('分享成功', 'success');
					}
				});
			},
			
			// 复制内容
			copyContent() {
				uni.setClipboardData({
					data: this.qrCodeContent,
					success: () => {
						this.showToastMessage('内容已复制', 'success');
					}
				});
			},
			
			// 缩放控制
			zoomIn() {
				if (this.scale < this.maxScale) {
					this.scale = Math.min(this.scale + this.scaleStep, this.maxScale);
				}
			},
			zoomOut() {
				if (this.scale > this.minScale) {
					this.scale = Math.max(this.scale - this.scaleStep, this.minScale);
				}
			},
			
			// 滚动监听
			onScroll(e) {
				this.scrollLeft = e.detail.scrollLeft;
				this.scrollTop = e.detail.scrollTop;
			},
			
			// 图片加载
			onImageLoad() {
				this.imageLoaded = true;
				console.log('二维码加载完成');
			},
			
			// 图片加载失败
			onImageError() {
				this.showToastMessage('二维码加载失败', 'error');
				this.imageLoaded = false;
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
			}
		}
	}
</script>

<style>
	/* 工业风配色 */
	:root {
		--industrial-bg: #1a1a1a;
		--industrial-card: #2d2d2d;
		--industrial-border: #3d3d3d;
		--industrial-text: #e0e0e0;
		--industrial-text-secondary: #888888;
		--industrial-accent: #ff6b35;
		--industrial-accent-blue: #4a9eff;
		--industrial-metal: #c0c0c0;
		--industrial-success: #4caf50;
		--industrial-error: #f44336;
	}

	.industrial-container {
		min-height: 100vh;
		background: linear-gradient(135deg, #1a1a1a 0%, #2d2d2d 100%);
		padding: 30rpx;
		box-sizing: border-box;
	}

	/* 顶部标题栏 */
	.header {
		display: flex;
		align-items: center;
		justify-content: center;
		margin-bottom: 30rpx;
		padding: 20rpx 0;
	}

	.header-line {
		flex: 1;
		height: 2rpx;
		background: linear-gradient(90deg, transparent, #ff6b35, transparent);
	}

	.header-title {
		font-size: 36rpx;
		font-weight: bold;
		color: #e0e0e0;
		margin: 0 30rpx;
		letter-spacing: 4rpx;
		text-transform: uppercase;
	}

	/* 信息卡片 */
	.info-card {
		background: linear-gradient(145deg, #2d2d2d, #252525);
		border: 2rpx solid #3d3d3d;
		border-radius: 16rpx;
		padding: 30rpx;
		margin-bottom: 30rpx;
		box-shadow: 0 8rpx 32rpx rgba(0, 0, 0, 0.3);
	}

	.info-row {
		display: flex;
		justify-content: space-between;
		align-items: center;
		margin-bottom: 20rpx;
	}

	.info-row:last-child {
		margin-bottom: 0;
	}

	.info-label {
		font-size: 28rpx;
		color: #888888;
		font-weight: 500;
	}

	.info-value {
		font-size: 32rpx;
		color: #e0e0e0;
		font-weight: bold;
	}

	.divider {
		height: 2rpx;
		background: linear-gradient(90deg, transparent, #3d3d3d, transparent);
		margin: 20rpx 0;
	}

	.status-indicator {
		display: flex;
		align-items: center;
		background: rgba(244, 67, 54, 0.2);
		padding: 8rpx 20rpx;
		border-radius: 8rpx;
		border: 2rpx solid #f44336;
	}

	.status-indicator.status-active {
		background: rgba(76, 175, 80, 0.2);
		border-color: #4caf50;
	}

	.status-dot {
		width: 16rpx;
		height: 16rpx;
		border-radius: 50%;
		background: #f44336;
		margin-right: 12rpx;
		animation: pulse 2s infinite;
	}

	.status-active .status-dot {
		background: #4caf50;
	}

	@keyframes pulse {
		0%, 100% { opacity: 1; }
		50% { opacity: 0.5; }
	}

	.status-text {
		font-size: 24rpx;
		color: #f44336;
		font-weight: 600;
	}

	.status-active .status-text {
		color: #4caf50;
	}

	/* 二维码区域 */
	.qrcode-section {
		background: linear-gradient(145deg, #2d2d2d, #252525);
		border: 2rpx solid #3d3d3d;
		border-radius: 16rpx;
		padding: 30rpx;
		margin-bottom: 30rpx;
		box-shadow: 0 8rpx 32rpx rgba(0, 0, 0, 0.3);
	}

	.section-header {
		display: flex;
		align-items: center;
		margin-bottom: 30rpx;
	}

	.section-icon {
		width: 8rpx;
		height: 32rpx;
		background: #ff6b35;
		margin-right: 16rpx;
		border-radius: 4rpx;
	}

	.section-title {
		font-size: 30rpx;
		color: #e0e0e0;
		font-weight: bold;
		margin-right: 20rpx;
	}

	.section-line {
		flex: 1;
		height: 2rpx;
		background: linear-gradient(90deg, #3d3d3d, transparent);
	}

	/* 二维码容器 */
	.qrcode-wrapper {
		display: flex;
		flex-direction: column;
		align-items: center;
	}

	.qrcode-border {
		position: relative;
		padding: 20rpx;
		background: #1a1a1a;
		border: 4rpx solid #3d3d3d;
		border-radius: 8rpx;
	}

	.corner {
		position: absolute;
		width: 30rpx;
		height: 30rpx;
		border-color: #ff6b35;
		border-style: solid;
		border-width: 0;
	}

	.corner-tl {
		top: -4rpx;
		left: -4rpx;
		border-top-width: 6rpx;
		border-left-width: 6rpx;
	}

	.corner-tr {
		top: -4rpx;
		right: -4rpx;
		border-top-width: 6rpx;
		border-right-width: 6rpx;
	}

	.corner-bl {
		bottom: -4rpx;
		left: -4rpx;
		border-bottom-width: 6rpx;
		border-left-width: 6rpx;
	}

	.corner-br {
		bottom: -4rpx;
		right: -4rpx;
		border-bottom-width: 6rpx;
		border-right-width: 6rpx;
	}

	.qrcode-scroll {
		width: 500rpx;
		height: 500rpx;
		background: #ffffff;
		overflow: hidden;
	}

	.qrcode-content {
		display: flex;
		justify-content: center;
		align-items: center;
		min-width: 100%;
		min-height: 100%;
		transition: transform 0.3s ease;
	}

	.qrcode-image {
		width: 400rpx;
		height: 400rpx;
		image-rendering: pixelated;
	}

	.qrcode-placeholder {
		display: flex;
		flex-direction: column;
		justify-content: center;
		align-items: center;
		width: 400rpx;
		height: 400rpx;
	}

	.loading-spinner {
		width: 60rpx;
		height: 60rpx;
		border: 6rpx solid #3d3d3d;
		border-top-color: #ff6b35;
		border-radius: 50%;
		animation: spin 1s linear infinite;
		margin-bottom: 20rpx;
	}

	@keyframes spin {
		0% { transform: rotate(0deg); }
		100% { transform: rotate(360deg); }
	}

	.placeholder-text {
		font-size: 28rpx;
		color: #888888;
	}

	/* 缩放控制 */
	.zoom-controls {
		display: flex;
		align-items: center;
		justify-content: center;
		margin-top: 30rpx;
		gap: 30rpx;
	}

	.zoom-btn {
		width: 80rpx;
		height: 80rpx;
		background: linear-gradient(145deg, #3d3d3d, #2d2d2d);
		border: 2rpx solid #4d4d4d;
		border-radius: 12rpx;
		display: flex;
		justify-content: center;
		align-items: center;
		transition: all 0.3s ease;
	}

	.zoom-btn:active {
		transform: scale(0.95);
		background: linear-gradient(145deg, #2d2d2d, #3d3d3d);
	}

	.zoom-btn.disabled {
		opacity: 0.5;
		pointer-events: none;
	}

	.zoom-icon {
		font-size: 40rpx;
		color: #e0e0e0;
		font-weight: bold;
	}

	.zoom-level {
		font-size: 28rpx;
		color: #ff6b35;
		font-weight: bold;
		min-width: 100rpx;
		text-align: center;
	}

	/* 二维码信息 */
	.qrcode-info {
		margin-top: 30rpx;
		padding: 20rpx;
		background: rgba(0, 0, 0, 0.3);
		border-radius: 8rpx;
		border-left: 6rpx solid #4a9eff;
	}

	.qrcode-label {
		display: block;
		font-size: 24rpx;
		color: #888888;
		margin-bottom: 10rpx;
	}

	.qrcode-value {
		font-size: 26rpx;
		color: #4a9eff;
		word-break: break-all;
		font-family: monospace;
	}

	/* 操作按钮区域 */
	.action-section {
		margin-bottom: 30rpx;
	}

	.action-row {
		display: flex;
		gap: 20rpx;
		margin-bottom: 20rpx;
	}

	.action-row:last-child {
		margin-bottom: 0;
	}

	.action-btn {
		flex: 1;
		display: flex;
		align-items: center;
		justify-content: center;
		padding: 30rpx 20rpx;
		border-radius: 12rpx;
		transition: all 0.3s ease;
		border: 2rpx solid;
	}

	.action-btn:active {
		transform: translateY(4rpx);
	}

	.btn-primary {
		background: linear-gradient(145deg, #ff6b35, #e55a2b);
		border-color: #ff6b35;
		box-shadow: 0 8rpx 20rpx rgba(255, 107, 53, 0.3);
	}

	.btn-primary:active {
		background: linear-gradient(145deg, #e55a2b, #ff6b35);
		box-shadow: 0 4rpx 10rpx rgba(255, 107, 53, 0.2);
	}

	.btn-secondary {
		background: linear-gradient(145deg, #4a9eff, #3a8eef);
		border-color: #4a9eff;
		box-shadow: 0 8rpx 20rpx rgba(74, 158, 255, 0.3);
	}

	.btn-secondary:active {
		background: linear-gradient(145deg, #3a8eef, #4a9eff);
		box-shadow: 0 4rpx 10rpx rgba(74, 158, 255, 0.2);
	}

	.btn-tertiary {
		background: linear-gradient(145deg, #3d3d3d, #2d2d2d);
		border-color: #4d4d4d;
		box-shadow: 0 8rpx 20rpx rgba(0, 0, 0, 0.3);
	}

	.btn-tertiary:active {
		background: linear-gradient(145deg, #2d2d2d, #3d3d3d);
		box-shadow: 0 4rpx 10rpx rgba(0, 0, 0, 0.2);
	}

	.btn-loading {
		opacity: 0.7;
		pointer-events: none;
	}

	.btn-icon {
		width: 40rpx;
		height: 40rpx;
		margin-right: 16rpx;
		display: flex;
		justify-content: center;
		align-items: center;
	}

	.refresh-icon {
		width: 32rpx;
		height: 32rpx;
		border: 4rpx solid #ffffff;
		border-top-color: transparent;
		border-radius: 50%;
	}

	.refresh-icon.rotating {
		animation: spin 1s linear infinite;
	}

	.save-icon {
		width: 28rpx;
		height: 32rpx;
		border: 4rpx solid #ffffff;
		border-radius: 4rpx;
		position: relative;
	}

	.save-icon::after {
		content: '';
		position: absolute;
		top: -8rpx;
		left: 50%;
		transform: translateX(-50%);
		width: 16rpx;
		height: 8rpx;
		background: #ffffff;
		border-radius: 2rpx;
	}

	.share-icon {
		width: 32rpx;
		height: 32rpx;
		border: 4rpx solid #e0e0e0;
		border-radius: 50%;
		position: relative;
	}

	.share-icon::before,
	.share-icon::after {
		content: '';
		position: absolute;
		width: 12rpx;
		height: 12rpx;
		background: #2d2d2d;
		border: 4rpx solid #e0e0e0;
		border-radius: 50%;
	}

	.share-icon::before {
		top: -6rpx;
		left: -6rpx;
	}

	.share-icon::after {
		bottom: -6rpx;
		right: -6rpx;
	}

	.copy-icon {
		width: 28rpx;
		height: 32rpx;
		border: 4rpx solid #e0e0e0;
		border-radius: 4rpx;
		position: relative;
	}

	.copy-icon::before {
		content: '';
		position: absolute;
		top: -8rpx;
		right: -8rpx;
		width: 20rpx;
		height: 24rpx;
		border: 4rpx solid #e0e0e0;
		border-radius: 4rpx;
		background: #2d2d2d;
	}

	.btn-text {
		font-size: 28rpx;
		color: #ffffff;
		font-weight: 600;
	}

	.btn-tertiary .btn-text {
		color: #e0e0e0;
	}

	/* 底部 */
	.footer {
		display: flex;
		align-items: center;
		justify-content: center;
		padding: 30rpx 0;
	}

	.footer-line {
		flex: 1;
		height: 2rpx;
		background: linear-gradient(90deg, transparent, #3d3d3d, transparent);
	}

	.footer-text {
		font-size: 24rpx;
		color: #666666;
		margin: 0 20rpx;
		letter-spacing: 2rpx;
	}

	/* 提示消息 */
	.toast-message {
		position: fixed;
		top: 50%;
		left: 50%;
		transform: translate(-50%, -50%) scale(0.8);
		background: rgba(45, 45, 45, 0.95);
		border: 2rpx solid #3d3d3d;
		border-radius: 16rpx;
		padding: 40rpx 60rpx;
		display: flex;
		flex-direction: column;
		align-items: center;
		opacity: 0;
		pointer-events: none;
		transition: all 0.3s ease;
		z-index: 9999;
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
		background: #4caf50;
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
		background: #f44336;
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
		color: #e0e0e0;
		font-weight: 500;
	}
</style>