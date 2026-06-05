<script>
	export default {
		onLaunch: function() {
			console.log('App Launch');
			this.$dbUtils.openDb('dataDB');
			this.$dbUtils.init('dataDB', [{
					tableName: 'data',
					sql: `CREATE TABLE "data" (
								"id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
								  "scancode" TEXT,
								  "ctime" timestamp  DEFAULT (datetime(CURRENT_TIMESTAMP,'localtime')),
								  "isUpdate" INTEGER DEFAULT 0
								  );`
				},
				{
					tableName: 'mes',
					sql: `CREATE TABLE "mes" (
								"id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
								  "gx" TEXT,
								  "jt" TEXT,
								  device_no TEXT,
								  deviceType TEXT,
								  "sb" TEXT,
								  "isOpen" INTEGER,
								  "ip" TEXT,
								  "port" TEXT DEFAULT '5193',
								  "apiPath" TEXT DEFAULT 'api/pda/scan');`
				}
			]);
			
			// 检查并添加缺失的字段（数据库迁移）
			this.checkAndAddColumns();
		},
		methods: {
			// 检查并添加缺失的字段
			checkAndAddColumns() {
				// 检查 mes 表是否有 port 字段
				this.$dbUtils.selectDataList('dataDB', 'mes', {id: 1}, 'id', 'desc').then(res => {
					console.log('检查数据库字段...');
				}).catch(err => {
					console.log('数据库查询错误，可能需要添加新字段');
				});
				
				// 尝试添加 deviceType 字段（如果不存在则添加）
				plus.sqlite.executeSql({
					name: 'dataDB',
					sql: `ALTER TABLE mes ADD COLUMN deviceType TEXT DEFAULT ''`,
					success: (e) => {
						console.log('成功添加 deviceType 字段');
					},
					fail: (e) => {
						// 字段已存在会报错，忽略错误
						console.log('deviceType 字段已存在或添加失败:', e.message);
					}
				});
				
				
				
				// 尝试添加 port 字段（如果不存在则添加）
				plus.sqlite.executeSql({
					name: 'dataDB',
					sql: `ALTER TABLE mes ADD COLUMN port TEXT DEFAULT '5193'`,
					success: (e) => {
						console.log('成功添加 port 字段');
					},
					fail: (e) => {
						// 字段已存在会报错，忽略错误
						console.log('port 字段已存在或添加失败:', e.message);
					}
				});
				
				// 尝试添加 apiPath 字段（如果不存在则添加）
				plus.sqlite.executeSql({
					name: 'dataDB',
					sql: `ALTER TABLE mes ADD COLUMN apiPath TEXT DEFAULT 'api/pda/scan'`,
					success: (e) => {
						console.log('成功添加 apiPath 字段');
					},
					fail: (e) => {
						// 字段已存在会报错，忽略错误
						console.log('apiPath 字段已存在或添加失败:', e.message);
					}
				});
			}
		},
		onShow: function() {
			console.log('App Show')
		},
		onHide: function() {
			console.log('App Hide')
		}
	}
</script>

<style>
	/*每个页面公共css */
</style>