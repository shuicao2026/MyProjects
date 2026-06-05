import App from './App'
import dbUtils from '/uni_modules/zjy-sqlite-manage/components/zjy-sqlite-manage/dbUtils.js'
// #ifndef VUE3
import Vue from 'vue'
import './uni.promisify.adaptor'
Vue.config.productionTip = false
App.mpType = 'app'
const app = new Vue({
	...App
})
app.$mount()
// #endif

// #ifdef VUE3
import {
	createSSRApp
} from 'vue'
export function createApp() {
	const app = createSSRApp(App)
	app.config.globalProperties.$dbUtils = dbUtils;
	return {
		app
	}
}
// #endif