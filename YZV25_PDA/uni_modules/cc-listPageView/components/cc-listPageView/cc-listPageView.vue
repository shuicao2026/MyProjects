<template>

	<view>

		<view style="display: flex; margin-top: 20px;  justify-content: center;">

			<view class="pageUpBtn" v-if="curPageNum > 1" @click="pageChangeClick(0)"> 上一页</view>
			<view v-if="totalNum !== '0'"
				style="font-size: 13px; margin-left: 0px; width: 180px; margin-top: 6px; text-align: center;">
				{{'第 ' + (curPageNum) + ' / ' + (Math.ceil(totalNum/pageCount)) + ' 页' + '&nbsp;&nbsp;  共 ' + totalNum + ' 条数据' }}
			</view>

			<view class="pageUpBtn" v-if="curPageNum < (Math.ceil(totalNum/pageCount))" @click="pageChangeClick(1)"
				style="margin-left: 0px;"> 下一页</view>

		</view>

		<view style="height: 80px;"></view>
	</view>

</template>

<script>
	export default {
		props: {

			//  分页单页数据数量
			pageCount: {
				type: [Number, String],
				default: 10
			},
			// 数据总数
			totalNum: {

				type: [Number, String],
				default: 0
			},
			// 当前页码
			curPageNum: {

				type: [Number, String],
				default: 1
			}


		},
		data() {
			
			return{
				
			}
		},
		methods: {

			pageChangeClick(tag) {

				this.$emit('pageClick', tag)

			},

		}
	}
</script>

<style scoped>
	.pageUpBtn {

		margin-left: 0px;
		background-color: white;
		border-radius: 4px;
		border-width: 1px;
		border-color: #999999;
		padding: 4px 6px;
		color: #333333;
		font-size: 15px;
	}
</style>