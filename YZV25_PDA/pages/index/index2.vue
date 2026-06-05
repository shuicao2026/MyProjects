<template>
	<view class="history-container" :class="['theme-' + currentTheme]">
		<!-- 自定义导航栏 -->
		<view class="custom-nav">
			<view class="nav-left" @click="goBack">
				<view class="nav-icon back-icon"></view>
				<text class="nav-text">返回</text>
			</view>
			<text class="nav-title">历史数据</text>
			<view class="nav-right"></view>
		</view>
		
		<!-- 主内容区域 -->
		<view class="main-content">
			<!-- 搜索栏 -->
			<view class="search-wrapper">
				<uni-search-bar radius="5" placeholder="输入要搜索的内容" clearButton="always" cancel-text="搜索" @cancel="searchPress" />
			</view>
			
			<!-- 列表区域 -->
			<scroll-view class="list-scroll" scroll-y>
				<uni-list class="myList">
					<uni-list-item class="myItem" v-for="(item, index) in dataList" :key="index" :title="item.scancode"
						:note="item.ctime" :ellipsis=1 clickable @click="onClick(item)" />
				</uni-list>
				<view v-if="dataList.length === 0" class="empty-tip">
					<text>暂无历史数据</text>
				</view>
			</scroll-view>
			
			<!-- 底部区域 -->
			<view class="bottom-area">
				<uni-pagination class="myPagination" :current="currentPage" :total="totalCount" :page-size="pageSize"
					@change="pageChange" />
				<view class="btn-view">
					<text class="example-info">当前页：{{ currentPage }}，数据总量：{{ totalCount }}条，每页数据：{{ pageSize }}</text>
				</view>
			</view>
		</view>
	</view>
</template>

<script>
	export default {
		data() {
			return {
				dataList: [],
				totalCount: 0,
				currentPage: 1,
				pageSize: 10,
				currentTheme: 'light'
			}
		},
		onLoad() {
			this.loadThemeFromStorage();
			this.requestData();
		},
		onPullDownRefresh() {
			this.requestData();
			setTimeout(function() {
				uni.stopPullDownRefresh();
			}, 300);
		},
		methods: {
			loadThemeFromStorage() {
				const savedTheme = uni.getStorageSync('appTheme');
				if (savedTheme) {
					this.currentTheme = savedTheme;
				}
			},
			goBack() {
				uni.navigateBack();
			},
			requestData() {
				this.$dbUtils.getCount('dataDB', 'data').then(res => {
					console.log('数据总量:', res);
					if (res && res.length > 0) {
						this.totalCount = res[0].num || 0;
					}
				}).catch(err => {
					console.log('获取数据总量失败:', err);
				});
				
				this.$dbUtils.getDataList('dataDB', 'data', this.currentPage, 10, 'id', 'desc')
					.then(res => {
						console.log('历史数据列表:', res);
						this.dataList = res || [];
					}).catch(err => {
						console.log('获取历史数据失败:', err);
						this.dataList = [];
					});
			},
			pageChange(e) {
				this.currentPage = e.current;
				this.requestData();
			},
			searchPress(searchValue) {
				this.$dbUtils.selectDataListByLike('dataDB', 'data', 'scancode', searchValue.value, 'id', 'desc')
					.then(res => {
						console.log('搜索结果:', res.length);
						this.currentPage = 1;
						this.totalCount = res.length;
						this.dataList = res || [];
					});
				uni.showToast({
					title: '正在查询：' + searchValue.value,
					icon: 'none'
				});
			},
			onClick(item, e) {
				uni.showModal({
					title: '数据',
					content: JSON.stringify(item),
					showCancel: false
				});
			},
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
		--shadow: rgba(255, 107, 53, 0.15);
	}

	.history-container {
		min-height: 100vh;
		background: var(--bg-primary);
	}

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

	.main-content {
		padding: 160rpx 20rpx 20rpx;
		display: flex;
		flex-direction: column;
		min-height: calc(100vh - 160rpx);
	}

	.search-wrapper {
		margin-bottom: 20rpx;
	}

	.list-scroll {
		flex: 1;
		min-height: 300rpx;
	}

	.bottom-area {
		background: var(--bg-card);
		border-radius: 12rpx;
		padding: 20rpx;
		margin-top: 20rpx;
	}

	.myList {
		background: var(--bg-card);
		border-radius: 12rpx;
	}

	.myItem {
		height: 80rpx;
		overflow: hidden;
		word-break: break-all;
		text-overflow: ellipsis;
		display: -webkit-box;
		-webkit-box-orient: vertical;
		-webkit-line-clamp: 1;
	}

	.empty-tip {
		text-align: center;
		padding: 60rpx 0;
		color: var(--text-secondary);
	}

	.btn-view {
		display: flex;
		flex-direction: column;
		padding: 10rpx 0 0;
		text-align: center;
		justify-content: center;
		align-items: center;
	}

	.myPagination {
		padding-left: 10%;
		padding-right: 10%;
	}
</style>