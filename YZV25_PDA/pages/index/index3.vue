<template>
	<view class="container" :class="['theme-' + currentTheme]">
		<!-- 自定义导航栏 -->
		<view class="custom-nav">
			<view class="nav-left" @click="goBack">
				<view class="nav-icon back-icon"></view>
				<text class="nav-text">返回</text>
			</view>
			<text class="nav-title">设置</text>
			<view class="nav-right"></view>
		</view>
		
		<view class="content-wrapper">
			<view class="example">
			<!-- 基础用法，不包含校验规则 -->
			<uni-forms ref="valiForm" :rules="rules" :modelValue="valiFormData">
				<uni-forms-item label="机台号" required name="gx">
					<uni-easyinput v-model="valiFormData.gx" placeholder="机台号" />
				</uni-forms-item>
				<uni-forms-item label="机台名称" required name="jt">
					<uni-easyinput v-model="valiFormData.jt" placeholder="机台名称" />
				</uni-forms-item>
				<uni-forms-item label="设备类型" required name="deviceType">
					 <picker 
					       
					    :range="options" 
						@change="onPickerChange"
					    class="picker"
					    >
					      <view class="picker-text">
					         {{ valiFormData.deviceType|| '请选择' }}
					      </view>
					    </picker>
				</uni-forms-item>
				<uni-forms-item label="本地服务器IP" required name="ip">
					<uni-easyinput v-model="valiFormData.ip" placeholder="请输入本地服务器IP" />
				</uni-forms-item>
				<uni-forms-item label="端口" required name="port">
					<uni-easyinput v-model="valiFormData.port" placeholder="请输入端口，如：6060" />
				</uni-forms-item>
				<uni-forms-item label="接口名称" required name="apiPath">
					<uni-easyinput v-model="valiFormData.apiPath" placeholder="请输入接口名称，如：api/pda/scan" />
				</uni-forms-item>
				
				<!-- 主题选择 -->
				<view class="theme-section">
					<text class="theme-label">主题</text>
					<view class="theme-selector">
						<view 
							class="theme-option" 
							:class="{'active': currentTheme === 'light'}"
							@click.stop="switchTheme('light')"
						>
							<view class="theme-preview theme-light-preview"></view>
							<text class="theme-name">浅色</text>
						</view>
						<view 
							class="theme-option" 
							:class="{'active': currentTheme === 'dark'}"
							@click.stop="switchTheme('dark')"
						>
							<view class="theme-preview theme-dark-preview"></view>
							<text class="theme-name">深色</text>
						</view>
						<view 
							class="theme-option" 
							:class="{'active': currentTheme === 'orange'}"
							@click.stop="switchTheme('orange')"
						>
							<view class="theme-preview theme-orange-preview"></view>
							<text class="theme-name">工业橙</text>
						</view>
					</view>
				</view>
			</uni-forms>

			<button type="primary" @click="submit('valiForm')">提交</button>
			</view>
		</view>
	</view>
</template>

<script>
	export default {
		data() {
			return {
				// 基础表单数据
				valiFormData: {
					gx: '1',
					jt: '',
					sb: 'PDA',
					isOpen: 0,
					ip: '',
					deviceType:"",
					port: '6060',
					apiPath: 'api/pda/scan'
				},
				// 校验规则
				rules: {
					gx: {
						rules: [{
							required: true,
							errorMessage: '工序不能为空'
						}]
					},
					jt: {
						rules: [{
							required: true,
							errorMessage: '机台不能为空'
						}]
					},
					sb: {
						rules: [{
							required: true,
							errorMessage: '设备不能为空'
						}]
					},
					ip: {
						rules: [{
							required: true,
							errorMessage: '本地服务器IP不能为空'
						}]
					},
					port: {
						rules: [{
							required: true,
							errorMessage: '端口不能为空'
						}]
					},
					deviceType:{
						rules: [{
							required: true,
							errorMessage: '设备类型不能为空'
						}]
					},
					apiPath: {
						rules: [{
							required: true,
							errorMessage: '接口名称不能为空'
						}]
					}
				},
				isOpens: [{
					"value": 0,
					"text": "关闭"
				}, {
					"value": 1,
					"text": "打开"
				}],
				options: ['CNC', 'CMT', 'FSW'],
				currentTheme: 'light'
			}
		},

		onLoad() {
			this.$dbUtils.selectDataList('dataDB', 'mes', {
				id: 1
			}, 'id', 'desc').then(res => {
				if (res.length == 0) {
					this.$dbUtils.addTabItem('dataDB', 'mes', {
						gx: "测试",
						jt: "测试",
						sb: "测试",
						isOpen: 0,
						deviceType:"",
						ip: "192.168.60.36",
						port: "6060",
						apiPath: "api/pda/scan",
					}).then(
						res3 => {
							this.valiFormData.gx = "1";
							this.valiFormData.jt = "测试";
							this.valiFormData.sb = "测试";
							this.valiFormData.isOpen = 0;
							this.valiFormData.deviceType="";
							this.valiFormData.ip = "192.168.60.36";
							this.valiFormData.port = "6060";
							this.valiFormData.apiPath = 'api/pda/scan';
						});
				} else {
					console.log(res[0])
					this.valiFormData.gx = res[0].gx;
					this.valiFormData.jt = res[0].jt;
					this.valiFormData.deviceType = res[0].deviceType;
					this.valiFormData.sb = res[0].sb;
					this.valiFormData.isOpen = res[0].isOpen;
					this.valiFormData.ip = res[0].ip;
					this.valiFormData.port = res[0].port || '6060';
					this.valiFormData.apiPath = res[0].apiPath || 'api/pda/scan';
				}
			});
			
			// 加载主题设置
			const savedTheme = uni.getStorageSync('appTheme');
			if (savedTheme) {
				this.currentTheme = savedTheme;
			}
		},
		methods: {
			onPickerChange(e){
				const index = e.detail.value;
				this.valiFormData.deviceType=this.options[index];
				
		
			},
			// 返回上一页
			goBack() {
				uni.navigateBack({
					delta: 1,
					fail: () => {
						// 如果返回失败，说明没有上一页，跳转到首页
						uni.reLaunch({
							url: '/pages/index/index'
						});
					}
				});
			},
			// 切换主题
			switchTheme(theme) {
				this.currentTheme = theme;
				uni.setStorageSync('appTheme', theme);
				uni.showToast({
					title: '主题已切换',
					icon: 'success'
				});
			},
			submit(ref) {
				this.$refs[ref].validate().then(res => {
					console.log('表单校验通过：'+JSON.stringify(res) )
					// 合并所有字段
					const updateData = {
						...res,
						isOpen: this.valiFormData.isOpen,
						sb: this.valiFormData.sb
					};
					console.log('更新数据：'+JSON.stringify(updateData) )
					
					// 先检查是否存在记录
					this.$dbUtils.selectDataList('dataDB', 'mes', {id: 1}, 'id', 'desc').then(existRes => {
						if (existRes.length > 0) {
							// 存在记录，执行更新
							this.$dbUtils.updateSQL('dataDB', 'mes', updateData, 'id', 1).then(
								res3 => {
									uni.showToast({
										title: '保存成功'
									});
									// 返回上一页
									setTimeout(() => {
										uni.navigateBack();
									}, 500);
								}).catch(err => {
									console.log('更新失败：', err);
									uni.showToast({
										title: '保存失败：' + (err.message || err),
										icon: 'none'
									});
								});
						} else {
							// 不存在记录，执行插入
							this.$dbUtils.addTabItem('dataDB', 'mes', {...updateData, id: 1}).then(
								res3 => {
									uni.showToast({
										title: '保存成功'
									});
									// 返回上一页
									setTimeout(() => {
										uni.navigateBack();
									}, 500);
								}).catch(err => {
									console.log('插入失败：', err);
									uni.showToast({
										title: '保存失败：' + (err.message || err),
										icon: 'none'
									});
								});
						}
					}).catch(err => {
						console.log('查询失败：', err);
						uni.showToast({
							title: '查询失败',
							icon: 'none'
						});
					});
				}).catch(err => {
					console.log('表单校验失败：', err);
					uni.showToast({
						title: '请检查输入内容',
						icon: 'none'
					});
				})
			}
		}
	}
</script>

<style lang="scss">
	/* ==================== 浅色主题 ==================== */
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
	.container {
		min-height: 100vh;
		background: var(--bg-primary);
		transition: all 0.3s ease;
	}

	/* 自定义导航栏 */
	.custom-nav {
		position: fixed;
		top: 40rpx;
		left: 0;
		right: 0;
		height: 90rpx;
		padding: 0 20rpx;
		background: var(--bg-card);
		border-bottom: 2rpx solid var(--border-color);
		display: flex;
		align-items: center;
		justify-content: space-between;
		z-index: 100;
	}

	.nav-left, .nav-right {
		width: 120rpx;
		display: flex;
		align-items: center;
	}

	.nav-right {
		justify-content: flex-end;
	}

	.nav-title {
		font-size: 34rpx;
		color: var(--text-primary);
		font-weight: bold;
	}

	.nav-text {
		font-size: 26rpx;
		color: var(--accent-color);
	}

	.nav-icon {
		width: 32rpx;
		height: 32rpx;
		margin-right: 8rpx;
	}

	.back-icon {
		border: 4rpx solid var(--accent-color);
		border-right: none;
		border-top: none;
		transform: rotate(45deg);
	}

	.content-wrapper {
		padding: 160rpx 20rpx 20rpx;
	}

	.example {
		padding: 15px;
		background: var(--bg-card);
		border: 2rpx solid var(--border-color);
		border-radius: 16rpx;
		margin: 20rpx;
	}

	.segmented-control {
		margin-bottom: 15px;
	}

	.button-group {
		margin-top: 15px;
		display: flex;
		justify-content: space-around;
	}

	.form-item {
		display: flex;
		align-items: center;
	}

	.button {
		display: flex;
		align-items: center;
		height: 35px;
		margin-left: 10px;
	}

	/* 防止标签换行 */
	:deep(.uni-forms-item__label) {
		white-space: nowrap !important;
		min-width: 80rpx !important;
	}

	/* 主题区域 */
	.theme-section {
		margin: 20rpx 0;
		padding: 0 15px;
	}

	.theme-label {
		font-size: 28rpx;
		color: var(--text-secondary);
		margin-bottom: 16rpx;
		display: block;
	}

	/* 主题选择器 */
	.theme-selector {
		display: flex;
		gap: 20rpx;
		padding: 10rpx 0;
	}

	.theme-option {
		flex: 1;
		display: flex;
		flex-direction: column;
		align-items: center;
		padding: 20rpx;
		border-radius: 12rpx;
		border: 2rpx solid var(--border-color);
		background: var(--bg-secondary);
		transition: all 0.3s ease;
	}

	.theme-option:active {
		transform: scale(0.95);
	}

	.theme-option.active {
		border-color: var(--accent-color);
		box-shadow: 0 0 0 2rpx var(--accent-color);
		background: rgba(64, 158, 255, 0.1);
	}

	.theme-preview {
		width: 60rpx;
		height: 60rpx;
		border-radius: 50%;
		margin-bottom: 12rpx;
		border: 4rpx solid #dcdfe6;
	}

	.theme-light-preview {
		background: linear-gradient(135deg, #f5f7fa, #ffffff);
	}

	.theme-dark-preview {
		background: linear-gradient(135deg, #1a1a2e, #16213e);
	}

	.theme-orange-preview {
		background: linear-gradient(135deg, #fff9f0, #ff6b35);
	}

	.theme-name {
		font-size: 24rpx;
		color: #606266;
		font-weight: 500;
	}
</style>
