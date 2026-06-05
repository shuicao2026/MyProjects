if (typeof Promise !== "undefined" && !Promise.prototype.finally) {
  Promise.prototype.finally = function(callback) {
    const promise = this.constructor;
    return this.then(
      (value) => promise.resolve(callback()).then(() => value),
      (reason) => promise.resolve(callback()).then(() => {
        throw reason;
      })
    );
  };
}
;
if (typeof uni !== "undefined" && uni && uni.requireGlobal) {
  const global2 = uni.requireGlobal();
  ArrayBuffer = global2.ArrayBuffer;
  Int8Array = global2.Int8Array;
  Uint8Array = global2.Uint8Array;
  Uint8ClampedArray = global2.Uint8ClampedArray;
  Int16Array = global2.Int16Array;
  Uint16Array = global2.Uint16Array;
  Int32Array = global2.Int32Array;
  Uint32Array = global2.Uint32Array;
  Float32Array = global2.Float32Array;
  Float64Array = global2.Float64Array;
  BigInt64Array = global2.BigInt64Array;
  BigUint64Array = global2.BigUint64Array;
}
;
if (uni.restoreGlobal) {
  uni.restoreGlobal(Vue, weex, plus, setTimeout, clearTimeout, setInterval, clearInterval);
}
(function(vue) {
  "use strict";
  function formatAppLog(type, filename, ...args) {
    if (uni.__log__) {
      uni.__log__(type, filename, ...args);
    } else {
      console[type].apply(console, [...args, filename]);
    }
  }
  function resolveEasycom(component, easycom) {
    return typeof component === "string" ? easycom : component;
  }
  const _export_sfc = (sfc, props) => {
    const target = sfc.__vccOpts || sfc;
    for (const [key, val] of props) {
      target[key] = val;
    }
    return target;
  };
  var main, receiver, filter;
  var codeQueryTag = false;
  const _sfc_main$d = {
    data() {
      return {
        scanCode: ""
      };
    },
    created() {
      this.initScan();
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
        formatAppLog("log", "at uni_modules/xw-scan/components/xw-scan/xw-scan.vue:112", "initScan");
        let that = this;
        main = plus.android.runtimeMainActivity();
        var IntentFilter = plus.android.importClass("android.content.IntentFilter");
        filter = new IntentFilter();
        filter.addAction("android.intent.action.SCANRESULT");
        receiver = plus.android.implements("io.dcloud.feature.internal.reflect.BroadcastReceiver", {
          onReceive: (context, intent) => {
            formatAppLog("log", "at uni_modules/xw-scan/components/xw-scan/xw-scan.vue:123", "onReceive");
            plus.android.importClass(intent);
            let code = intent.getStringExtra("value");
            that.queryCode(code);
          }
        });
      },
      startScan() {
        formatAppLog("log", "at uni_modules/xw-scan/components/xw-scan/xw-scan.vue:136", "startScan");
        main.registerReceiver(receiver, filter);
      },
      stopScan() {
        formatAppLog("log", "at uni_modules/xw-scan/components/xw-scan/xw-scan.vue:142", "stopScan");
        main.unregisterReceiver(receiver);
      },
      queryCode: function(code) {
        formatAppLog("log", "at uni_modules/xw-scan/components/xw-scan/xw-scan.vue:148", "queryCode");
        if (codeQueryTag)
          return false;
        codeQueryTag = true;
        setTimeout(function() {
          codeQueryTag = false;
        }, 150);
        var id = code;
        uni.$emit("xwscan", {
          code: id
        });
      }
    }
  };
  function _sfc_render$c(_ctx, _cache, $props, $setup, $data, $options) {
    return vue.openBlock(), vue.createElementBlock("view", null, [
      vue.createElementVNode("view", { class: "content" })
    ]);
  }
  const __easycom_0$3 = /* @__PURE__ */ _export_sfc(_sfc_main$d, [["render", _sfc_render$c], ["__file", "E:/NiuNiu/code/YZV25_PDA/uni_modules/xw-scan/components/xw-scan/xw-scan.vue"]]);
  const _sfc_main$c = {
    data() {
      return {
        results: [],
        timer: null,
        ip: "",
        id: "",
        jt: "",
        deviceType: "",
        port: "6060",
        apiPath: "api/pda/scan",
        faceType: "A",
        scanLock: false,
        expandedItems: {},
        showToast: false,
        toastMessage: "",
        toastType: "success",
        toastTimer: null,
        currentTheme: "light",
        // 密码验证弹窗相关
        showPwdModal: false,
        barcode: "",
        pwdInput: "",
        selectedFace: "A"
      };
    },
    onLoad() {
      this.loadConfig();
      this.loadThemeFromStorage();
    },
    onUnload() {
      formatAppLog("log", "at pages/index/index.vue:178", "onUnload");
      uni.$off("xwscan");
    },
    onHide() {
      formatAppLog("log", "at pages/index/index.vue:182", "onHide");
    },
    methods: {
      // 从本地存储加载主题
      loadThemeFromStorage() {
        const savedTheme = uni.getStorageSync("appTheme");
        if (savedTheme) {
          this.currentTheme = savedTheme;
        }
      },
      // 加载配置
      loadConfig() {
        this.$dbUtils.selectDataList("dataDB", "mes", { id: 1 }, "id", "desc").then((res) => {
          if (res.length >= 1) {
            this.ip = res[0].ip;
            this.id = res[0].gx;
            this.jt = res[0].jt;
            this.deviceType = res[0].deviceType;
            this.port = res[0].port || "6060";
            this.apiPath = res[0].apiPath || "api/pda/scan";
            uni.setNavigationBarTitle({ title: res[0].jt });
          } else {
            this.ip = "192.168.60.36";
          }
        });
      },
      // 生成唯一ID
      generateId() {
        return Date.now() + "_" + Math.floor(Math.random() * 1e3);
      },
      // 根据ID更新状态
      updateStatusById(id, status, color, errorInfo = null) {
        const targetIndex = this.results.findIndex((item) => item.id === id);
        if (targetIndex !== -1) {
          this.results.splice(targetIndex, 1, {
            ...this.results[targetIndex],
            status,
            color,
            errorInfo
          });
        }
      },
      // 显示错误详情
      showErrorDetail(errorInfo) {
        let detail = "";
        if (typeof errorInfo === "object") {
          detail = JSON.stringify(errorInfo, null, 2);
        } else {
          detail = errorInfo;
        }
        uni.showModal({
          title: "错误详情",
          content: detail,
          showCancel: false,
          confirmText: "知道了",
          confirmColor: "#409eff"
        });
      },
      // 获取徽章样式类
      getBadgeClass(index) {
        if (index === 0)
          return "badge-current";
        if (index === 1)
          return "badge-second";
        if (index === 2)
          return "badge-third";
        return "badge-normal";
      },
      // 复制条码
      copyBarcode(code) {
        uni.setClipboardData({
          data: code,
          success: () => {
            this.showToastMessage("条码已复制", "success");
          }
        });
      },
      // 显示提示
      showToastMessage(message, type = "success") {
        this.toastMessage = message;
        this.toastType = type;
        this.showToast = true;
        if (this.toastTimer) {
          clearTimeout(this.toastTimer);
        }
        this.toastTimer = setTimeout(() => {
          this.showToast = false;
        }, 2e3);
      },
      // 跳转到设置页面
      goToSetting() {
        uni.navigateTo({
          url: "index3",
          fail: (err) => {
            formatAppLog("log", "at pages/index/index.vue:283", "跳转失败:", err);
            uni.showToast({
              title: "页面跳转失败",
              icon: "none"
            });
          }
        });
      },
      // 跳转到历史记录页面
      goToHistory() {
        uni.navigateTo({
          url: "index2",
          fail: (err) => {
            formatAppLog("log", "at pages/index/index.vue:297", "跳转失败:", err);
            uni.showToast({
              title: "页面跳转失败",
              icon: "none"
            });
          }
        });
      },
      // 显示密码验证弹窗
      showConfirmPwdModal() {
        this.showPwdModal = true;
        this.pwdInput = "";
        this.selectedFace = "A";
      },
      // 关闭密码验证弹窗
      closePwdModal() {
        this.showPwdModal = false;
        this.pwdInput = "";
      },
      // 确认密码
      confirmPwd() {
        if (!this.pwdInput) {
          this.showToastMessage("请输入密码", "error");
          return;
        }
        uni.showLoading({ title: "验证中...", mask: true });
        const fullApiUrl = "http://" + this.ip + ":" + this.port + "/api/pda/force";
        const requestData = {
          id: this.id,
          code: this.barcode,
          pwd: this.pwdInput,
          face: this.selectedFace
        };
        formatAppLog("log", "at pages/index/index.vue:337", "========== 密码验证信息 ==========");
        formatAppLog("log", "at pages/index/index.vue:338", "路由地址:", fullApiUrl);
        formatAppLog("log", "at pages/index/index.vue:339", "验证数据:", JSON.stringify(requestData));
        formatAppLog("log", "at pages/index/index.vue:340", "==================================");
        uni.request({
          url: fullApiUrl,
          method: "POST",
          data: JSON.stringify(requestData),
          header: { "content-type": "application/json" },
          success: (res) => {
            formatAppLog("log", "at pages/index/index.vue:348", "密码验证返回：", res.data);
            if (res.data) {
              formatAppLog("log", "at pages/index/index.vue:351", "res.data:", res.data);
              let msg = this.getJson(res.data);
              formatAppLog("log", "at pages/index/index.vue:353", msg);
              if (msg.code == 0) {
                this.showToastMessage(msg.msg || "操作成功", "success");
                this.closePwdModal();
              } else {
                this.showToastMessage(msg.msg || "密码错误", "error");
              }
            } else {
              this.showToastMessage("服务器返回空数据", "error");
            }
          },
          fail: (err) => {
            formatAppLog("log", "at pages/index/index.vue:365", "密码验证失败：", err);
            this.showToastMessage("服务器连接失败", "error");
          },
          complete: () => {
            uni.hideLoading();
          }
        });
      },
      getJson(obj) {
        if (typeof str !== "string")
          return obj;
        try {
          const obj2 = JSON.parse(str);
          return obj2;
        } catch (e) {
          return obj;
        }
      }
    },
    onShow() {
      this.loadConfig();
      uni.$off("xwscan");
      uni.$on("xwscan", (res) => {
        if (this.showPwdModal) {
          return;
        }
        const nowcode = res.code;
        if (this.scanLock) {
          const shakeItem = {
            id: this.generateId(),
            code: nowcode,
            status: "抖动",
            color: "#909399"
          };
          if (this.results.length >= 10) {
            this.results.pop();
          }
          this.results.unshift(shakeItem);
          formatAppLog("log", "at pages/index/index.vue:411", "锁定中，本次扫码标记为抖动：", nowcode);
          return;
        }
        const currentId = this.generateId();
        const currentItem = {
          id: currentId,
          code: nowcode,
          status: "上传中",
          color: "#409EFF"
        };
        this.scanLock = true;
        if (this.results.length >= 10) {
          this.results.pop();
        }
        this.results.unshift(currentItem);
        uni.showLoading({ title: "处理中...", mask: true });
        this.$dbUtils.addTabItem("dataDB", "data", { scancode: nowcode }).then((res2) => {
          formatAppLog("log", "at pages/index/index.vue:434", "扫码数据已保存到数据库:", nowcode);
        }).catch((err) => {
          formatAppLog("log", "at pages/index/index.vue:436", "保存到数据库失败:", err);
        });
        let timeoutTimer = null;
        timeoutTimer = setTimeout(() => {
          this.updateStatusById(currentId, "响应超时", "#E6A23C");
          this.scanLock = false;
          uni.hideLoading();
          this.showToastMessage("服务器响应超时", "error");
        }, 5e3);
        res.id = this.id;
        res.face = this.faceType;
        const fullApiUrl = "http://" + this.ip + ":" + this.port + "/" + this.apiPath;
        formatAppLog("log", "at pages/index/index.vue:455", "========== 扫码上传信息 ==========");
        formatAppLog("log", "at pages/index/index.vue:456", "路由地址:", fullApiUrl);
        formatAppLog("log", "at pages/index/index.vue:457", "扫码内容:", nowcode);
        formatAppLog("log", "at pages/index/index.vue:458", "==================================");
        uni.request({
          url: fullApiUrl,
          method: "POST",
          data: JSON.stringify(res),
          header: { "content-type": "application/json" },
          success: (res2) => {
            formatAppLog("log", "at pages/index/index.vue:466", "服务器返回：", res2.data);
            if (res2.data) {
              let msg = this.getJson(res2.data);
              formatAppLog("log", "at pages/index/index.vue:469", "服务器返回msg：", currentId, msg.code);
              if (msg.code == 0) {
                this.updateStatusById(currentId, "上传成功", "#67C23A");
                this.showToastMessage("上传成功", "success");
              } else {
                this.updateStatusById(currentId, "上传失败", "#F56C6C", {
                  code: msg.code,
                  message: msg.msg || "上传失败"
                });
                this.showToastMessage(msg.msg || "上传失败", "error");
              }
            } else {
              this.updateStatusById(currentId, "上传失败", "#F56C6C", {
                message: "服务器返回空数据"
              });
            }
          },
          fail: (res3) => {
            formatAppLog("log", "at pages/index/index.vue:487", "请求失败：", res3.errMsg);
            this.updateStatusById(currentId, "上传失败", "#F56C6C", {
              message: "服务器连接失败",
              errMsg: res3.errMsg,
              statusCode: res3.statusCode
            });
            this.showToastMessage("服务器连接失败", "error");
          },
          complete: () => {
            clearTimeout(timeoutTimer);
            this.scanLock = false;
            uni.hideLoading();
          }
        });
      });
    }
  };
  function _sfc_render$b(_ctx, _cache, $props, $setup, $data, $options) {
    const _component_xw_scan = resolveEasycom(vue.resolveDynamicComponent("xw-scan"), __easycom_0$3);
    return vue.openBlock(), vue.createElementBlock(
      "view",
      {
        class: vue.normalizeClass(["scan-container", ["theme-" + $data.currentTheme]])
      },
      [
        vue.createCommentVNode(" 扫描组件 "),
        vue.createVNode(_component_xw_scan),
        vue.createCommentVNode(" 自定义导航栏 "),
        vue.createElementVNode("view", { class: "custom-nav" }, [
          vue.createElementVNode("view", {
            class: "nav-left",
            onClick: _cache[0] || (_cache[0] = (...args) => $options.goToHistory && $options.goToHistory(...args))
          }, [
            vue.createElementVNode("view", { class: "nav-icon history-icon-small" }),
            vue.createElementVNode("text", { class: "nav-text" }, "历史")
          ]),
          vue.createElementVNode(
            "text",
            { class: "nav-title" },
            vue.toDisplayString($data.jt || "V25 工作站"),
            1
            /* TEXT */
          ),
          vue.createElementVNode("view", {
            class: "nav-right",
            onClick: _cache[1] || (_cache[1] = (...args) => $options.goToSetting && $options.goToSetting(...args))
          }, [
            vue.createElementVNode("text", { class: "nav-text" }, "设置"),
            vue.createElementVNode("view", { class: "nav-icon setting-icon-small" })
          ])
        ]),
        vue.createCommentVNode(" 扫码结果列表 "),
        vue.createElementVNode("view", { class: "scan-list" }, [
          vue.createElementVNode("view", { class: "list-header" }, [
            vue.createElementVNode("view", { class: "header-accent" }),
            vue.createElementVNode("text", { class: "header-title" }, "扫描记录"),
            vue.createElementVNode(
              "text",
              { class: "header-count" },
              vue.toDisplayString($data.results.length) + "/10",
              1
              /* TEXT */
            )
          ]),
          vue.createElementVNode("scroll-view", {
            class: "list-scroll",
            "scroll-y": "true"
          }, [
            (vue.openBlock(true), vue.createElementBlock(
              vue.Fragment,
              null,
              vue.renderList($data.results, (item, index) => {
                return vue.openBlock(), vue.createElementBlock(
                  "view",
                  {
                    class: vue.normalizeClass(["scan-item", { "current": index === 0, "uploading": item.status === "上传中" }]),
                    key: item.id
                  },
                  [
                    vue.createCommentVNode(" 顶部栏：序号 + 状态 + 操作 "),
                    vue.createElementVNode("view", { class: "item-top-bar" }, [
                      vue.createElementVNode(
                        "view",
                        {
                          class: vue.normalizeClass(["item-badge", $options.getBadgeClass(index)])
                        },
                        [
                          vue.createElementVNode(
                            "text",
                            { class: "badge-number" },
                            vue.toDisplayString(index + 1),
                            1
                            /* TEXT */
                          )
                        ],
                        2
                        /* CLASS */
                      ),
                      vue.createElementVNode("view", { class: "status-group" }, [
                        vue.createElementVNode(
                          "view",
                          {
                            class: "status-tag",
                            style: vue.normalizeStyle({ backgroundColor: item.color + "20", borderColor: item.color, color: item.color })
                          },
                          [
                            vue.createElementVNode(
                              "text",
                              { class: "tag-text" },
                              vue.toDisplayString(item.status),
                              1
                              /* TEXT */
                            )
                          ],
                          4
                          /* STYLE */
                        ),
                        item.status === "上传失败" && item.errorInfo ? (vue.openBlock(), vue.createElementBlock("view", {
                          key: 0,
                          class: "error-detail-btn",
                          onClick: ($event) => $options.showErrorDetail(item.errorInfo),
                          title: "查看错误详情"
                        }, [
                          vue.createElementVNode("view", { class: "error-detail-icon" }),
                          vue.createElementVNode("text", { class: "error-detail-text" }, "详情")
                        ], 8, ["onClick"])) : vue.createCommentVNode("v-if", true)
                      ]),
                      vue.createElementVNode("view", { class: "action-group" }, [
                        index === 0 ? (vue.openBlock(), vue.createElementBlock("view", {
                          key: 0,
                          class: "action-icon",
                          onClick: ($event) => $options.copyBarcode(item.code),
                          title: "复制"
                        }, [
                          vue.createElementVNode("view", { class: "icon-copy" })
                        ], 8, ["onClick"])) : vue.createCommentVNode("v-if", true)
                      ])
                    ]),
                    vue.createCommentVNode(" 条码内容 - 无框显示 "),
                    vue.createElementVNode("view", { class: "barcode-content" }, [
                      vue.createElementVNode(
                        "text",
                        {
                          class: "barcode-text",
                          selectable: ""
                        },
                        vue.toDisplayString(item.code),
                        1
                        /* TEXT */
                      )
                    ])
                  ],
                  2
                  /* CLASS */
                );
              }),
              128
              /* KEYED_FRAGMENT */
            )),
            vue.createCommentVNode(" 空状态 "),
            $data.results.length === 0 ? (vue.openBlock(), vue.createElementBlock("view", {
              key: 0,
              class: "empty-state"
            }, [
              vue.createElementVNode("view", { class: "empty-icon" }, [
                vue.createElementVNode("view", { class: "scan-icon" })
              ]),
              vue.createElementVNode("text", { class: "empty-title" }, "等待扫描"),
              vue.createElementVNode("text", { class: "empty-desc" }, "请使用扫码枪扫描条码")
            ])) : vue.createCommentVNode("v-if", true)
          ])
        ]),
        vue.createCommentVNode(" 提示消息 "),
        $data.showToast ? (vue.openBlock(), vue.createElementBlock(
          "view",
          {
            key: 0,
            class: vue.normalizeClass(["toast-message", { "toast-show": $data.showToast }])
          },
          [
            vue.createElementVNode(
              "view",
              {
                class: vue.normalizeClass(["toast-icon", $data.toastType])
              },
              null,
              2
              /* CLASS */
            ),
            vue.createElementVNode(
              "text",
              { class: "toast-text" },
              vue.toDisplayString($data.toastMessage),
              1
              /* TEXT */
            )
          ],
          2
          /* CLASS */
        )) : vue.createCommentVNode("v-if", true),
        vue.createCommentVNode(" 底部强制完成按钮 "),
        vue.createElementVNode("view", { class: "bottom-action" }, [
          vue.createElementVNode("view", {
            class: "force-complete-btn",
            onClick: _cache[2] || (_cache[2] = (...args) => $options.showConfirmPwdModal && $options.showConfirmPwdModal(...args))
          }, [
            vue.createElementVNode("text", { class: "btn-text" }, "强制条码出站")
          ])
        ]),
        vue.createCommentVNode(" 密码验证弹窗 "),
        $data.showPwdModal ? (vue.openBlock(), vue.createElementBlock("view", {
          key: 1,
          class: "modal-overlay",
          onClick: _cache[10] || (_cache[10] = (...args) => $options.closePwdModal && $options.closePwdModal(...args))
        }, [
          vue.createElementVNode("view", {
            class: "modal-content",
            onClick: _cache[9] || (_cache[9] = vue.withModifiers(() => {
            }, ["stop"]))
          }, [
            vue.createElementVNode("view", { class: "modal-header" }, [
              vue.createElementVNode("text", { class: "modal-title" }, "强制条码出站")
            ]),
            vue.createElementVNode("view", { class: "modal-body" }, [
              vue.createElementVNode("view", { class: "pwd-input-wrapper" }, [
                vue.withDirectives(vue.createElementVNode(
                  "input",
                  {
                    focus: true,
                    class: "pwd-input",
                    type: "text",
                    "onUpdate:modelValue": _cache[3] || (_cache[3] = ($event) => $data.barcode = $event),
                    placeholder: "请输入条码,空则为该PDA最新扫的条码",
                    "placeholder-class": "pwd-placeholder"
                  },
                  null,
                  512
                  /* NEED_PATCH */
                ), [
                  [vue.vModelText, $data.barcode]
                ])
              ]),
              vue.createElementVNode("view", { class: "pwd-input-wrapper" }, [
                vue.withDirectives(vue.createElementVNode(
                  "input",
                  {
                    class: "pwd-input",
                    type: "password",
                    "onUpdate:modelValue": _cache[4] || (_cache[4] = ($event) => $data.pwdInput = $event),
                    placeholder: "请输入密码",
                    "placeholder-class": "pwd-placeholder"
                  },
                  null,
                  512
                  /* NEED_PATCH */
                ), [
                  [vue.vModelText, $data.pwdInput]
                ])
              ]),
              $data.deviceType === "CMT" ? (vue.openBlock(), vue.createElementBlock("view", {
                key: 0,
                class: "face-selector"
              }, [
                vue.createElementVNode(
                  "view",
                  {
                    class: vue.normalizeClass(["face-option", { "active": $data.selectedFace === "A" }]),
                    onClick: _cache[5] || (_cache[5] = ($event) => $data.selectedFace = "A")
                  },
                  [
                    vue.createElementVNode(
                      "view",
                      {
                        class: vue.normalizeClass(["face-radio", { "checked": $data.selectedFace === "A" }])
                      },
                      null,
                      2
                      /* CLASS */
                    ),
                    vue.createElementVNode("text", { class: "face-text" }, "A面")
                  ],
                  2
                  /* CLASS */
                ),
                vue.createElementVNode(
                  "view",
                  {
                    class: vue.normalizeClass(["face-option", { "active": $data.selectedFace === "B" }]),
                    onClick: _cache[6] || (_cache[6] = ($event) => $data.selectedFace = "B")
                  },
                  [
                    vue.createElementVNode(
                      "view",
                      {
                        class: vue.normalizeClass(["face-radio", { "checked": $data.selectedFace === "B" }])
                      },
                      null,
                      2
                      /* CLASS */
                    ),
                    vue.createElementVNode("text", { class: "face-text" }, "B面")
                  ],
                  2
                  /* CLASS */
                )
              ])) : vue.createCommentVNode("v-if", true)
            ]),
            vue.createElementVNode("view", { class: "modal-footer" }, [
              vue.createElementVNode("view", {
                class: "modal-btn modal-btn-cancel",
                onClick: _cache[7] || (_cache[7] = (...args) => $options.closePwdModal && $options.closePwdModal(...args))
              }, [
                vue.createElementVNode("text", { class: "btn-label" }, "取消")
              ]),
              vue.createElementVNode("view", {
                class: "modal-btn modal-btn-confirm",
                onClick: _cache[8] || (_cache[8] = (...args) => $options.confirmPwd && $options.confirmPwd(...args))
              }, [
                vue.createElementVNode("text", { class: "btn-label" }, "确定")
              ])
            ])
          ])
        ])) : vue.createCommentVNode("v-if", true)
      ],
      2
      /* CLASS */
    );
  }
  const PagesIndexIndex = /* @__PURE__ */ _export_sfc(_sfc_main$c, [["render", _sfc_render$b], ["__file", "E:/NiuNiu/code/YZV25_PDA/pages/index/index.vue"]]);
  const fontData = [
    {
      "font_class": "arrow-down",
      "unicode": ""
    },
    {
      "font_class": "arrow-left",
      "unicode": ""
    },
    {
      "font_class": "arrow-right",
      "unicode": ""
    },
    {
      "font_class": "arrow-up",
      "unicode": ""
    },
    {
      "font_class": "auth",
      "unicode": ""
    },
    {
      "font_class": "auth-filled",
      "unicode": ""
    },
    {
      "font_class": "back",
      "unicode": ""
    },
    {
      "font_class": "bars",
      "unicode": ""
    },
    {
      "font_class": "calendar",
      "unicode": ""
    },
    {
      "font_class": "calendar-filled",
      "unicode": ""
    },
    {
      "font_class": "camera",
      "unicode": ""
    },
    {
      "font_class": "camera-filled",
      "unicode": ""
    },
    {
      "font_class": "cart",
      "unicode": ""
    },
    {
      "font_class": "cart-filled",
      "unicode": ""
    },
    {
      "font_class": "chat",
      "unicode": ""
    },
    {
      "font_class": "chat-filled",
      "unicode": ""
    },
    {
      "font_class": "chatboxes",
      "unicode": ""
    },
    {
      "font_class": "chatboxes-filled",
      "unicode": ""
    },
    {
      "font_class": "chatbubble",
      "unicode": ""
    },
    {
      "font_class": "chatbubble-filled",
      "unicode": ""
    },
    {
      "font_class": "checkbox",
      "unicode": ""
    },
    {
      "font_class": "checkbox-filled",
      "unicode": ""
    },
    {
      "font_class": "checkmarkempty",
      "unicode": ""
    },
    {
      "font_class": "circle",
      "unicode": ""
    },
    {
      "font_class": "circle-filled",
      "unicode": ""
    },
    {
      "font_class": "clear",
      "unicode": ""
    },
    {
      "font_class": "close",
      "unicode": ""
    },
    {
      "font_class": "closeempty",
      "unicode": ""
    },
    {
      "font_class": "cloud-download",
      "unicode": ""
    },
    {
      "font_class": "cloud-download-filled",
      "unicode": ""
    },
    {
      "font_class": "cloud-upload",
      "unicode": ""
    },
    {
      "font_class": "cloud-upload-filled",
      "unicode": ""
    },
    {
      "font_class": "color",
      "unicode": ""
    },
    {
      "font_class": "color-filled",
      "unicode": ""
    },
    {
      "font_class": "compose",
      "unicode": ""
    },
    {
      "font_class": "contact",
      "unicode": ""
    },
    {
      "font_class": "contact-filled",
      "unicode": ""
    },
    {
      "font_class": "down",
      "unicode": ""
    },
    {
      "font_class": "bottom",
      "unicode": ""
    },
    {
      "font_class": "download",
      "unicode": ""
    },
    {
      "font_class": "download-filled",
      "unicode": ""
    },
    {
      "font_class": "email",
      "unicode": ""
    },
    {
      "font_class": "email-filled",
      "unicode": ""
    },
    {
      "font_class": "eye",
      "unicode": ""
    },
    {
      "font_class": "eye-filled",
      "unicode": ""
    },
    {
      "font_class": "eye-slash",
      "unicode": ""
    },
    {
      "font_class": "eye-slash-filled",
      "unicode": ""
    },
    {
      "font_class": "fire",
      "unicode": ""
    },
    {
      "font_class": "fire-filled",
      "unicode": ""
    },
    {
      "font_class": "flag",
      "unicode": ""
    },
    {
      "font_class": "flag-filled",
      "unicode": ""
    },
    {
      "font_class": "folder-add",
      "unicode": ""
    },
    {
      "font_class": "folder-add-filled",
      "unicode": ""
    },
    {
      "font_class": "font",
      "unicode": ""
    },
    {
      "font_class": "forward",
      "unicode": ""
    },
    {
      "font_class": "gear",
      "unicode": ""
    },
    {
      "font_class": "gear-filled",
      "unicode": ""
    },
    {
      "font_class": "gift",
      "unicode": ""
    },
    {
      "font_class": "gift-filled",
      "unicode": ""
    },
    {
      "font_class": "hand-down",
      "unicode": ""
    },
    {
      "font_class": "hand-down-filled",
      "unicode": ""
    },
    {
      "font_class": "hand-up",
      "unicode": ""
    },
    {
      "font_class": "hand-up-filled",
      "unicode": ""
    },
    {
      "font_class": "headphones",
      "unicode": ""
    },
    {
      "font_class": "heart",
      "unicode": ""
    },
    {
      "font_class": "heart-filled",
      "unicode": ""
    },
    {
      "font_class": "help",
      "unicode": ""
    },
    {
      "font_class": "help-filled",
      "unicode": ""
    },
    {
      "font_class": "home",
      "unicode": ""
    },
    {
      "font_class": "home-filled",
      "unicode": ""
    },
    {
      "font_class": "image",
      "unicode": ""
    },
    {
      "font_class": "image-filled",
      "unicode": ""
    },
    {
      "font_class": "images",
      "unicode": ""
    },
    {
      "font_class": "images-filled",
      "unicode": ""
    },
    {
      "font_class": "info",
      "unicode": ""
    },
    {
      "font_class": "info-filled",
      "unicode": ""
    },
    {
      "font_class": "left",
      "unicode": ""
    },
    {
      "font_class": "link",
      "unicode": ""
    },
    {
      "font_class": "list",
      "unicode": ""
    },
    {
      "font_class": "location",
      "unicode": ""
    },
    {
      "font_class": "location-filled",
      "unicode": ""
    },
    {
      "font_class": "locked",
      "unicode": ""
    },
    {
      "font_class": "locked-filled",
      "unicode": ""
    },
    {
      "font_class": "loop",
      "unicode": ""
    },
    {
      "font_class": "mail-open",
      "unicode": ""
    },
    {
      "font_class": "mail-open-filled",
      "unicode": ""
    },
    {
      "font_class": "map",
      "unicode": ""
    },
    {
      "font_class": "map-filled",
      "unicode": ""
    },
    {
      "font_class": "map-pin",
      "unicode": ""
    },
    {
      "font_class": "map-pin-ellipse",
      "unicode": ""
    },
    {
      "font_class": "medal",
      "unicode": ""
    },
    {
      "font_class": "medal-filled",
      "unicode": ""
    },
    {
      "font_class": "mic",
      "unicode": ""
    },
    {
      "font_class": "mic-filled",
      "unicode": ""
    },
    {
      "font_class": "micoff",
      "unicode": ""
    },
    {
      "font_class": "micoff-filled",
      "unicode": ""
    },
    {
      "font_class": "minus",
      "unicode": ""
    },
    {
      "font_class": "minus-filled",
      "unicode": ""
    },
    {
      "font_class": "more",
      "unicode": ""
    },
    {
      "font_class": "more-filled",
      "unicode": ""
    },
    {
      "font_class": "navigate",
      "unicode": ""
    },
    {
      "font_class": "navigate-filled",
      "unicode": ""
    },
    {
      "font_class": "notification",
      "unicode": ""
    },
    {
      "font_class": "notification-filled",
      "unicode": ""
    },
    {
      "font_class": "paperclip",
      "unicode": ""
    },
    {
      "font_class": "paperplane",
      "unicode": ""
    },
    {
      "font_class": "paperplane-filled",
      "unicode": ""
    },
    {
      "font_class": "person",
      "unicode": ""
    },
    {
      "font_class": "person-filled",
      "unicode": ""
    },
    {
      "font_class": "personadd",
      "unicode": ""
    },
    {
      "font_class": "personadd-filled",
      "unicode": ""
    },
    {
      "font_class": "personadd-filled-copy",
      "unicode": ""
    },
    {
      "font_class": "phone",
      "unicode": ""
    },
    {
      "font_class": "phone-filled",
      "unicode": ""
    },
    {
      "font_class": "plus",
      "unicode": ""
    },
    {
      "font_class": "plus-filled",
      "unicode": ""
    },
    {
      "font_class": "plusempty",
      "unicode": ""
    },
    {
      "font_class": "pulldown",
      "unicode": ""
    },
    {
      "font_class": "pyq",
      "unicode": ""
    },
    {
      "font_class": "qq",
      "unicode": ""
    },
    {
      "font_class": "redo",
      "unicode": ""
    },
    {
      "font_class": "redo-filled",
      "unicode": ""
    },
    {
      "font_class": "refresh",
      "unicode": ""
    },
    {
      "font_class": "refresh-filled",
      "unicode": ""
    },
    {
      "font_class": "refreshempty",
      "unicode": ""
    },
    {
      "font_class": "reload",
      "unicode": ""
    },
    {
      "font_class": "right",
      "unicode": ""
    },
    {
      "font_class": "scan",
      "unicode": ""
    },
    {
      "font_class": "search",
      "unicode": ""
    },
    {
      "font_class": "settings",
      "unicode": ""
    },
    {
      "font_class": "settings-filled",
      "unicode": ""
    },
    {
      "font_class": "shop",
      "unicode": ""
    },
    {
      "font_class": "shop-filled",
      "unicode": ""
    },
    {
      "font_class": "smallcircle",
      "unicode": ""
    },
    {
      "font_class": "smallcircle-filled",
      "unicode": ""
    },
    {
      "font_class": "sound",
      "unicode": ""
    },
    {
      "font_class": "sound-filled",
      "unicode": ""
    },
    {
      "font_class": "spinner-cycle",
      "unicode": ""
    },
    {
      "font_class": "staff",
      "unicode": ""
    },
    {
      "font_class": "staff-filled",
      "unicode": ""
    },
    {
      "font_class": "star",
      "unicode": ""
    },
    {
      "font_class": "star-filled",
      "unicode": ""
    },
    {
      "font_class": "starhalf",
      "unicode": ""
    },
    {
      "font_class": "trash",
      "unicode": ""
    },
    {
      "font_class": "trash-filled",
      "unicode": ""
    },
    {
      "font_class": "tune",
      "unicode": ""
    },
    {
      "font_class": "tune-filled",
      "unicode": ""
    },
    {
      "font_class": "undo",
      "unicode": ""
    },
    {
      "font_class": "undo-filled",
      "unicode": ""
    },
    {
      "font_class": "up",
      "unicode": ""
    },
    {
      "font_class": "top",
      "unicode": ""
    },
    {
      "font_class": "upload",
      "unicode": ""
    },
    {
      "font_class": "upload-filled",
      "unicode": ""
    },
    {
      "font_class": "videocam",
      "unicode": ""
    },
    {
      "font_class": "videocam-filled",
      "unicode": ""
    },
    {
      "font_class": "vip",
      "unicode": ""
    },
    {
      "font_class": "vip-filled",
      "unicode": ""
    },
    {
      "font_class": "wallet",
      "unicode": ""
    },
    {
      "font_class": "wallet-filled",
      "unicode": ""
    },
    {
      "font_class": "weibo",
      "unicode": ""
    },
    {
      "font_class": "weixin",
      "unicode": ""
    }
  ];
  const getVal = (val) => {
    const reg = /^[0-9]*$/g;
    return typeof val === "number" || reg.test(val) ? val + "px" : val;
  };
  const _sfc_main$b = {
    name: "UniIcons",
    emits: ["click"],
    props: {
      type: {
        type: String,
        default: ""
      },
      color: {
        type: String,
        default: "#333333"
      },
      size: {
        type: [Number, String],
        default: 16
      },
      customPrefix: {
        type: String,
        default: ""
      },
      fontFamily: {
        type: String,
        default: ""
      }
    },
    data() {
      return {
        icons: fontData
      };
    },
    computed: {
      unicode() {
        let code = this.icons.find((v) => v.font_class === this.type);
        if (code) {
          return code.unicode;
        }
        return "";
      },
      iconSize() {
        return getVal(this.size);
      },
      styleObj() {
        if (this.fontFamily !== "") {
          return `color: ${this.color}; font-size: ${this.iconSize}; font-family: ${this.fontFamily};`;
        }
        return `color: ${this.color}; font-size: ${this.iconSize};`;
      }
    },
    methods: {
      _onClick() {
        this.$emit("click");
      }
    }
  };
  function _sfc_render$a(_ctx, _cache, $props, $setup, $data, $options) {
    return vue.openBlock(), vue.createElementBlock(
      "text",
      {
        style: vue.normalizeStyle($options.styleObj),
        class: vue.normalizeClass(["uni-icons", ["uniui-" + $props.type, $props.customPrefix, $props.customPrefix ? $props.type : ""]]),
        onClick: _cache[0] || (_cache[0] = (...args) => $options._onClick && $options._onClick(...args))
      },
      [
        vue.renderSlot(_ctx.$slots, "default", {}, void 0, true)
      ],
      6
      /* CLASS, STYLE */
    );
  }
  const __easycom_0$2 = /* @__PURE__ */ _export_sfc(_sfc_main$b, [["render", _sfc_render$a], ["__scopeId", "data-v-d31e1c47"], ["__file", "E:/NiuNiu/code/YZV25_PDA/uni_modules/uni-icons/components/uni-icons/uni-icons.vue"]]);
  const isObject = (val) => val !== null && typeof val === "object";
  const defaultDelimiters = ["{", "}"];
  class BaseFormatter {
    constructor() {
      this._caches = /* @__PURE__ */ Object.create(null);
    }
    interpolate(message, values, delimiters = defaultDelimiters) {
      if (!values) {
        return [message];
      }
      let tokens = this._caches[message];
      if (!tokens) {
        tokens = parse(message, delimiters);
        this._caches[message] = tokens;
      }
      return compile(tokens, values);
    }
  }
  const RE_TOKEN_LIST_VALUE = /^(?:\d)+/;
  const RE_TOKEN_NAMED_VALUE = /^(?:\w)+/;
  function parse(format, [startDelimiter, endDelimiter]) {
    const tokens = [];
    let position = 0;
    let text = "";
    while (position < format.length) {
      let char = format[position++];
      if (char === startDelimiter) {
        if (text) {
          tokens.push({ type: "text", value: text });
        }
        text = "";
        let sub = "";
        char = format[position++];
        while (char !== void 0 && char !== endDelimiter) {
          sub += char;
          char = format[position++];
        }
        const isClosed = char === endDelimiter;
        const type = RE_TOKEN_LIST_VALUE.test(sub) ? "list" : isClosed && RE_TOKEN_NAMED_VALUE.test(sub) ? "named" : "unknown";
        tokens.push({ value: sub, type });
      } else {
        text += char;
      }
    }
    text && tokens.push({ type: "text", value: text });
    return tokens;
  }
  function compile(tokens, values) {
    const compiled = [];
    let index = 0;
    const mode = Array.isArray(values) ? "list" : isObject(values) ? "named" : "unknown";
    if (mode === "unknown") {
      return compiled;
    }
    while (index < tokens.length) {
      const token = tokens[index];
      switch (token.type) {
        case "text":
          compiled.push(token.value);
          break;
        case "list":
          compiled.push(values[parseInt(token.value, 10)]);
          break;
        case "named":
          if (mode === "named") {
            compiled.push(values[token.value]);
          } else {
            {
              console.warn(`Type of token '${token.type}' and format of value '${mode}' don't match!`);
            }
          }
          break;
        case "unknown":
          {
            console.warn(`Detect 'unknown' type of token!`);
          }
          break;
      }
      index++;
    }
    return compiled;
  }
  const LOCALE_ZH_HANS = "zh-Hans";
  const LOCALE_ZH_HANT = "zh-Hant";
  const LOCALE_EN = "en";
  const LOCALE_FR = "fr";
  const LOCALE_ES = "es";
  const hasOwnProperty = Object.prototype.hasOwnProperty;
  const hasOwn = (val, key) => hasOwnProperty.call(val, key);
  const defaultFormatter = new BaseFormatter();
  function include(str2, parts) {
    return !!parts.find((part) => str2.indexOf(part) !== -1);
  }
  function startsWith(str2, parts) {
    return parts.find((part) => str2.indexOf(part) === 0);
  }
  function normalizeLocale(locale, messages2) {
    if (!locale) {
      return;
    }
    locale = locale.trim().replace(/_/g, "-");
    if (messages2 && messages2[locale]) {
      return locale;
    }
    locale = locale.toLowerCase();
    if (locale === "chinese") {
      return LOCALE_ZH_HANS;
    }
    if (locale.indexOf("zh") === 0) {
      if (locale.indexOf("-hans") > -1) {
        return LOCALE_ZH_HANS;
      }
      if (locale.indexOf("-hant") > -1) {
        return LOCALE_ZH_HANT;
      }
      if (include(locale, ["-tw", "-hk", "-mo", "-cht"])) {
        return LOCALE_ZH_HANT;
      }
      return LOCALE_ZH_HANS;
    }
    let locales = [LOCALE_EN, LOCALE_FR, LOCALE_ES];
    if (messages2 && Object.keys(messages2).length > 0) {
      locales = Object.keys(messages2);
    }
    const lang = startsWith(locale, locales);
    if (lang) {
      return lang;
    }
  }
  class I18n {
    constructor({ locale, fallbackLocale, messages: messages2, watcher, formater: formater2 }) {
      this.locale = LOCALE_EN;
      this.fallbackLocale = LOCALE_EN;
      this.message = {};
      this.messages = {};
      this.watchers = [];
      if (fallbackLocale) {
        this.fallbackLocale = fallbackLocale;
      }
      this.formater = formater2 || defaultFormatter;
      this.messages = messages2 || {};
      this.setLocale(locale || LOCALE_EN);
      if (watcher) {
        this.watchLocale(watcher);
      }
    }
    setLocale(locale) {
      const oldLocale = this.locale;
      this.locale = normalizeLocale(locale, this.messages) || this.fallbackLocale;
      if (!this.messages[this.locale]) {
        this.messages[this.locale] = {};
      }
      this.message = this.messages[this.locale];
      if (oldLocale !== this.locale) {
        this.watchers.forEach((watcher) => {
          watcher(this.locale, oldLocale);
        });
      }
    }
    getLocale() {
      return this.locale;
    }
    watchLocale(fn) {
      const index = this.watchers.push(fn) - 1;
      return () => {
        this.watchers.splice(index, 1);
      };
    }
    add(locale, message, override = true) {
      const curMessages = this.messages[locale];
      if (curMessages) {
        if (override) {
          Object.assign(curMessages, message);
        } else {
          Object.keys(message).forEach((key) => {
            if (!hasOwn(curMessages, key)) {
              curMessages[key] = message[key];
            }
          });
        }
      } else {
        this.messages[locale] = message;
      }
    }
    f(message, values, delimiters) {
      return this.formater.interpolate(message, values, delimiters).join("");
    }
    t(key, locale, values) {
      let message = this.message;
      if (typeof locale === "string") {
        locale = normalizeLocale(locale, this.messages);
        locale && (message = this.messages[locale]);
      } else {
        values = locale;
      }
      if (!hasOwn(message, key)) {
        console.warn(`Cannot translate the value of keypath ${key}. Use the value of keypath as default.`);
        return key;
      }
      return this.formater.interpolate(message[key], values).join("");
    }
  }
  function watchAppLocale(appVm, i18n) {
    if (appVm.$watchLocale) {
      appVm.$watchLocale((newLocale) => {
        i18n.setLocale(newLocale);
      });
    } else {
      appVm.$watch(() => appVm.$locale, (newLocale) => {
        i18n.setLocale(newLocale);
      });
    }
  }
  function getDefaultLocale() {
    if (typeof uni !== "undefined" && uni.getLocale) {
      return uni.getLocale();
    }
    if (typeof global !== "undefined" && global.getLocale) {
      return global.getLocale();
    }
    return LOCALE_EN;
  }
  function initVueI18n(locale, messages2 = {}, fallbackLocale, watcher) {
    if (typeof locale !== "string") {
      const options = [
        messages2,
        locale
      ];
      locale = options[0];
      messages2 = options[1];
    }
    if (typeof locale !== "string") {
      locale = getDefaultLocale();
    }
    if (typeof fallbackLocale !== "string") {
      fallbackLocale = typeof __uniConfig !== "undefined" && __uniConfig.fallbackLocale || LOCALE_EN;
    }
    const i18n = new I18n({
      locale,
      fallbackLocale,
      messages: messages2,
      watcher
    });
    let t2 = (key, values) => {
      if (typeof getApp !== "function") {
        t2 = function(key2, values2) {
          return i18n.t(key2, values2);
        };
      } else {
        let isWatchedAppLocale = false;
        t2 = function(key2, values2) {
          const appVm = getApp().$vm;
          if (appVm) {
            appVm.$locale;
            if (!isWatchedAppLocale) {
              isWatchedAppLocale = true;
              watchAppLocale(appVm, i18n);
            }
          }
          return i18n.t(key2, values2);
        };
      }
      return t2(key, values);
    };
    return {
      i18n,
      f(message, values, delimiters) {
        return i18n.f(message, values, delimiters);
      },
      t(key, values) {
        return t2(key, values);
      },
      add(locale2, message, override = true) {
        return i18n.add(locale2, message, override);
      },
      watch(fn) {
        return i18n.watchLocale(fn);
      },
      getLocale() {
        return i18n.getLocale();
      },
      setLocale(newLocale) {
        return i18n.setLocale(newLocale);
      }
    };
  }
  const en$1 = {
    "uni-search-bar.cancel": "cancel",
    "uni-search-bar.placeholder": "Search enter content"
  };
  const zhHans$1 = {
    "uni-search-bar.cancel": "取消",
    "uni-search-bar.placeholder": "请输入搜索内容"
  };
  const zhHant$1 = {
    "uni-search-bar.cancel": "取消",
    "uni-search-bar.placeholder": "請輸入搜索內容"
  };
  const messages$1 = {
    en: en$1,
    "zh-Hans": zhHans$1,
    "zh-Hant": zhHant$1
  };
  const {
    t: t$1
  } = initVueI18n(messages$1);
  const _sfc_main$a = {
    name: "UniSearchBar",
    emits: ["input", "update:modelValue", "clear", "cancel", "confirm", "blur", "focus"],
    props: {
      placeholder: {
        type: String,
        default: ""
      },
      radius: {
        type: [Number, String],
        default: 5
      },
      clearButton: {
        type: String,
        default: "auto"
      },
      cancelButton: {
        type: String,
        default: "auto"
      },
      cancelText: {
        type: String,
        default: ""
      },
      bgColor: {
        type: String,
        default: "#F8F8F8"
      },
      maxlength: {
        type: [Number, String],
        default: 100
      },
      value: {
        type: [Number, String],
        default: ""
      },
      modelValue: {
        type: [Number, String],
        default: ""
      },
      focus: {
        type: Boolean,
        default: false
      },
      readonly: {
        type: Boolean,
        default: false
      }
    },
    data() {
      return {
        show: false,
        showSync: false,
        searchVal: ""
      };
    },
    computed: {
      cancelTextI18n() {
        return this.cancelText || t$1("uni-search-bar.cancel");
      },
      placeholderText() {
        return this.placeholder || t$1("uni-search-bar.placeholder");
      }
    },
    watch: {
      modelValue: {
        immediate: true,
        handler(newVal) {
          this.searchVal = newVal;
          if (newVal) {
            this.show = true;
          }
        }
      },
      focus: {
        immediate: true,
        handler(newVal) {
          if (newVal) {
            if (this.readonly)
              return;
            this.show = true;
            this.$nextTick(() => {
              this.showSync = true;
            });
          }
        }
      },
      searchVal(newVal, oldVal) {
        this.$emit("input", newVal);
        this.$emit("update:modelValue", newVal);
      }
    },
    methods: {
      searchClick() {
        if (this.readonly)
          return;
        if (this.show) {
          return;
        }
        this.show = true;
        this.$nextTick(() => {
          this.showSync = true;
        });
      },
      clear() {
        this.searchVal = "";
        this.$emit("clear", "");
      },
      cancel() {
        if (this.readonly)
          return;
        this.$emit("cancel", {
          value: this.searchVal
        });
        this.searchVal = "";
        this.show = false;
        this.showSync = false;
        plus.key.hideSoftKeybord();
      },
      confirm() {
        plus.key.hideSoftKeybord();
        this.$emit("confirm", {
          value: this.searchVal
        });
      },
      blur() {
        plus.key.hideSoftKeybord();
        this.$emit("blur", {
          value: this.searchVal
        });
      },
      emitFocus(e) {
        this.$emit("focus", e.detail);
      }
    }
  };
  function _sfc_render$9(_ctx, _cache, $props, $setup, $data, $options) {
    const _component_uni_icons = resolveEasycom(vue.resolveDynamicComponent("uni-icons"), __easycom_0$2);
    return vue.openBlock(), vue.createElementBlock("view", { class: "uni-searchbar" }, [
      vue.createElementVNode(
        "view",
        {
          style: vue.normalizeStyle({ borderRadius: $props.radius + "px", backgroundColor: $props.bgColor }),
          class: "uni-searchbar__box",
          onClick: _cache[5] || (_cache[5] = (...args) => $options.searchClick && $options.searchClick(...args))
        },
        [
          vue.createElementVNode("view", { class: "uni-searchbar__box-icon-search" }, [
            vue.renderSlot(_ctx.$slots, "searchIcon", {}, () => [
              vue.createVNode(_component_uni_icons, {
                color: "#c0c4cc",
                size: "18",
                type: "search"
              })
            ], true)
          ]),
          $data.show || $data.searchVal ? vue.withDirectives((vue.openBlock(), vue.createElementBlock("input", {
            key: 0,
            focus: $data.showSync,
            disabled: $props.readonly,
            placeholder: $options.placeholderText,
            maxlength: $props.maxlength,
            class: "uni-searchbar__box-search-input",
            "confirm-type": "search",
            type: "text",
            "onUpdate:modelValue": _cache[0] || (_cache[0] = ($event) => $data.searchVal = $event),
            onConfirm: _cache[1] || (_cache[1] = (...args) => $options.confirm && $options.confirm(...args)),
            onBlur: _cache[2] || (_cache[2] = (...args) => $options.blur && $options.blur(...args)),
            onFocus: _cache[3] || (_cache[3] = (...args) => $options.emitFocus && $options.emitFocus(...args))
          }, null, 40, ["focus", "disabled", "placeholder", "maxlength"])), [
            [vue.vModelText, $data.searchVal]
          ]) : (vue.openBlock(), vue.createElementBlock(
            "text",
            {
              key: 1,
              class: "uni-searchbar__text-placeholder"
            },
            vue.toDisplayString($props.placeholder),
            1
            /* TEXT */
          )),
          $data.show && ($props.clearButton === "always" || $props.clearButton === "auto" && $data.searchVal !== "") && !$props.readonly ? (vue.openBlock(), vue.createElementBlock("view", {
            key: 2,
            class: "uni-searchbar__box-icon-clear",
            onClick: _cache[4] || (_cache[4] = (...args) => $options.clear && $options.clear(...args))
          }, [
            vue.renderSlot(_ctx.$slots, "clearIcon", {}, () => [
              vue.createVNode(_component_uni_icons, {
                color: "#c0c4cc",
                size: "20",
                type: "clear"
              })
            ], true)
          ])) : vue.createCommentVNode("v-if", true)
        ],
        4
        /* STYLE */
      ),
      $props.cancelButton === "always" || $data.show && $props.cancelButton === "auto" ? (vue.openBlock(), vue.createElementBlock(
        "text",
        {
          key: 0,
          onClick: _cache[6] || (_cache[6] = (...args) => $options.cancel && $options.cancel(...args)),
          class: "uni-searchbar__cancel"
        },
        vue.toDisplayString($options.cancelTextI18n),
        1
        /* TEXT */
      )) : vue.createCommentVNode("v-if", true)
    ]);
  }
  const __easycom_0$1 = /* @__PURE__ */ _export_sfc(_sfc_main$a, [["render", _sfc_render$9], ["__scopeId", "data-v-f07ef577"], ["__file", "E:/NiuNiu/code/YZV25_PDA/uni_modules/uni-search-bar/components/uni-search-bar/uni-search-bar.vue"]]);
  const _sfc_main$9 = {
    name: "UniBadge",
    emits: ["click"],
    props: {
      type: {
        type: String,
        default: "error"
      },
      inverted: {
        type: Boolean,
        default: false
      },
      isDot: {
        type: Boolean,
        default: false
      },
      maxNum: {
        type: Number,
        default: 99
      },
      absolute: {
        type: String,
        default: ""
      },
      offset: {
        type: Array,
        default() {
          return [0, 0];
        }
      },
      text: {
        type: [String, Number],
        default: ""
      },
      size: {
        type: String,
        default: "small"
      },
      customStyle: {
        type: Object,
        default() {
          return {};
        }
      }
    },
    data() {
      return {};
    },
    computed: {
      width() {
        return String(this.text).length * 8 + 12;
      },
      classNames() {
        const {
          inverted,
          type,
          size,
          absolute
        } = this;
        return [
          inverted ? "uni-badge--" + type + "-inverted" : "",
          "uni-badge--" + type,
          "uni-badge--" + size,
          absolute ? "uni-badge--absolute" : ""
        ].join(" ");
      },
      positionStyle() {
        if (!this.absolute)
          return {};
        let w = this.width / 2, h = 10;
        if (this.isDot) {
          w = 5;
          h = 5;
        }
        const x = `${-w + this.offset[0]}px`;
        const y = `${-h + this.offset[1]}px`;
        const whiteList = {
          rightTop: {
            right: x,
            top: y
          },
          rightBottom: {
            right: x,
            bottom: y
          },
          leftBottom: {
            left: x,
            bottom: y
          },
          leftTop: {
            left: x,
            top: y
          }
        };
        const match = whiteList[this.absolute];
        return match ? match : whiteList["rightTop"];
      },
      dotStyle() {
        if (!this.isDot)
          return {};
        return {
          width: "10px",
          minWidth: "0",
          height: "10px",
          padding: "0",
          borderRadius: "10px"
        };
      },
      displayValue() {
        const {
          isDot,
          text,
          maxNum
        } = this;
        return isDot ? "" : Number(text) > maxNum ? `${maxNum}+` : text;
      }
    },
    methods: {
      onClick() {
        this.$emit("click");
      }
    }
  };
  function _sfc_render$8(_ctx, _cache, $props, $setup, $data, $options) {
    return vue.openBlock(), vue.createElementBlock("view", { class: "uni-badge--x" }, [
      vue.renderSlot(_ctx.$slots, "default", {}, void 0, true),
      $props.text ? (vue.openBlock(), vue.createElementBlock(
        "text",
        {
          key: 0,
          class: vue.normalizeClass([$options.classNames, "uni-badge"]),
          style: vue.normalizeStyle([$options.positionStyle, $props.customStyle, $options.dotStyle]),
          onClick: _cache[0] || (_cache[0] = ($event) => $options.onClick())
        },
        vue.toDisplayString($options.displayValue),
        7
        /* TEXT, CLASS, STYLE */
      )) : vue.createCommentVNode("v-if", true)
    ]);
  }
  const __easycom_1$2 = /* @__PURE__ */ _export_sfc(_sfc_main$9, [["render", _sfc_render$8], ["__scopeId", "data-v-c97cb896"], ["__file", "E:/NiuNiu/code/YZV25_PDA/uni_modules/uni-badge/components/uni-badge/uni-badge.vue"]]);
  const _sfc_main$8 = {
    name: "UniListItem",
    emits: ["click", "switchChange"],
    props: {
      direction: {
        type: String,
        default: "row"
      },
      title: {
        type: String,
        default: ""
      },
      note: {
        type: String,
        default: ""
      },
      ellipsis: {
        type: [Number, String],
        default: 0
      },
      disabled: {
        type: [Boolean, String],
        default: false
      },
      clickable: {
        type: Boolean,
        default: false
      },
      showArrow: {
        type: [Boolean, String],
        default: false
      },
      link: {
        type: [Boolean, String],
        default: false
      },
      to: {
        type: String,
        default: ""
      },
      showBadge: {
        type: [Boolean, String],
        default: false
      },
      showSwitch: {
        type: [Boolean, String],
        default: false
      },
      switchChecked: {
        type: [Boolean, String],
        default: false
      },
      badgeText: {
        type: String,
        default: ""
      },
      badgeType: {
        type: String,
        default: "success"
      },
      badgeStyle: {
        type: Object,
        default() {
          return {};
        }
      },
      rightText: {
        type: String,
        default: ""
      },
      thumb: {
        type: String,
        default: ""
      },
      thumbSize: {
        type: String,
        default: "base"
      },
      showExtraIcon: {
        type: [Boolean, String],
        default: false
      },
      extraIcon: {
        type: Object,
        default() {
          return {
            type: "",
            color: "#000000",
            size: 20,
            customPrefix: ""
          };
        }
      },
      border: {
        type: Boolean,
        default: true
      },
      customStyle: {
        type: Object,
        default() {
          return {
            padding: "",
            backgroundColor: "#FFFFFF"
          };
        }
      },
      keepScrollPosition: {
        type: Boolean,
        default: false
      }
    },
    watch: {
      "customStyle.padding": {
        handler(padding) {
          if (typeof padding == "number") {
            padding += "";
          }
          let paddingArr = padding.split(" ");
          if (paddingArr.length === 1) {
            const allPadding = paddingArr[0];
            this.padding = {
              "top": allPadding,
              "right": allPadding,
              "bottom": allPadding,
              "left": allPadding
            };
          } else if (paddingArr.length === 2) {
            const [verticalPadding, horizontalPadding] = paddingArr;
            this.padding = {
              "top": verticalPadding,
              "right": horizontalPadding,
              "bottom": verticalPadding,
              "left": horizontalPadding
            };
          } else if (paddingArr.length === 4) {
            const [topPadding, rightPadding, bottomPadding, leftPadding] = paddingArr;
            this.padding = {
              "top": topPadding,
              "right": rightPadding,
              "bottom": bottomPadding,
              "left": leftPadding
            };
          }
        },
        immediate: true
      }
    },
    // inject: ['list'],
    data() {
      return {
        isFirstChild: false,
        padding: {
          top: "",
          right: "",
          bottom: "",
          left: ""
        }
      };
    },
    mounted() {
      this.list = this.getForm();
      if (this.list) {
        if (!this.list.firstChildAppend) {
          this.list.firstChildAppend = true;
          this.isFirstChild = true;
        }
      }
    },
    methods: {
      /**
       * 获取父元素实例
       */
      getForm(name = "uniList") {
        let parent = this.$parent;
        let parentName = parent.$options.name;
        while (parentName !== name) {
          parent = parent.$parent;
          if (!parent)
            return false;
          parentName = parent.$options.name;
        }
        return parent;
      },
      onClick() {
        if (this.to !== "") {
          this.openPage();
          return;
        }
        if (this.clickable || this.link) {
          this.$emit("click", {
            data: {}
          });
        }
      },
      onSwitchChange(e) {
        this.$emit("switchChange", e.detail);
      },
      openPage() {
        if (["navigateTo", "redirectTo", "reLaunch", "switchTab"].indexOf(this.link) !== -1) {
          this.pageApi(this.link);
        } else {
          this.pageApi("navigateTo");
        }
      },
      pageApi(api) {
        let callback = {
          url: this.to,
          success: (res) => {
            this.$emit("click", {
              data: res
            });
          },
          fail: (err) => {
            this.$emit("click", {
              data: err
            });
          }
        };
        switch (api) {
          case "navigateTo":
            uni.navigateTo(callback);
            break;
          case "redirectTo":
            uni.redirectTo(callback);
            break;
          case "reLaunch":
            uni.reLaunch(callback);
            break;
          case "switchTab":
            uni.switchTab(callback);
            break;
          default:
            uni.navigateTo(callback);
        }
      }
    }
  };
  function _sfc_render$7(_ctx, _cache, $props, $setup, $data, $options) {
    const _component_uni_icons = resolveEasycom(vue.resolveDynamicComponent("uni-icons"), __easycom_0$2);
    const _component_uni_badge = resolveEasycom(vue.resolveDynamicComponent("uni-badge"), __easycom_1$2);
    return vue.openBlock(), vue.createElementBlock("view", {
      class: vue.normalizeClass([{ "uni-list-item--disabled": $props.disabled }, "uni-list-item"]),
      style: vue.normalizeStyle({ "background-color": $props.customStyle.backgroundColor }),
      "hover-class": !$props.clickable && !$props.link || $props.disabled || $props.showSwitch ? "" : "uni-list-item--hover",
      onClick: _cache[1] || (_cache[1] = (...args) => $options.onClick && $options.onClick(...args))
    }, [
      !$data.isFirstChild ? (vue.openBlock(), vue.createElementBlock(
        "view",
        {
          key: 0,
          class: vue.normalizeClass(["border--left", { "uni-list--border": $props.border }])
        },
        null,
        2
        /* CLASS */
      )) : vue.createCommentVNode("v-if", true),
      vue.createElementVNode(
        "view",
        {
          class: vue.normalizeClass(["uni-list-item__container", { "container--right": $props.showArrow || $props.link, "flex--direction": $props.direction === "column" }]),
          style: vue.normalizeStyle({ paddingTop: $data.padding.top, paddingLeft: $data.padding.left, paddingRight: $data.padding.right, paddingBottom: $data.padding.bottom })
        },
        [
          vue.renderSlot(_ctx.$slots, "header", {}, () => [
            vue.createElementVNode("view", { class: "uni-list-item__header" }, [
              $props.thumb ? (vue.openBlock(), vue.createElementBlock("view", {
                key: 0,
                class: "uni-list-item__icon"
              }, [
                vue.createElementVNode("image", {
                  src: $props.thumb,
                  class: vue.normalizeClass(["uni-list-item__icon-img", ["uni-list--" + $props.thumbSize]])
                }, null, 10, ["src"])
              ])) : $props.showExtraIcon ? (vue.openBlock(), vue.createElementBlock("view", {
                key: 1,
                class: "uni-list-item__icon"
              }, [
                vue.createVNode(_component_uni_icons, {
                  customPrefix: $props.extraIcon.customPrefix,
                  color: $props.extraIcon.color,
                  size: $props.extraIcon.size,
                  type: $props.extraIcon.type
                }, null, 8, ["customPrefix", "color", "size", "type"])
              ])) : vue.createCommentVNode("v-if", true)
            ])
          ], true),
          vue.renderSlot(_ctx.$slots, "body", {}, () => [
            vue.createElementVNode(
              "view",
              {
                class: vue.normalizeClass(["uni-list-item__content", { "uni-list-item__content--center": $props.thumb || $props.showExtraIcon || $props.showBadge || $props.showSwitch }])
              },
              [
                $props.title ? (vue.openBlock(), vue.createElementBlock(
                  "text",
                  {
                    key: 0,
                    class: vue.normalizeClass(["uni-list-item__content-title", [$props.ellipsis !== 0 && $props.ellipsis <= 2 ? "uni-ellipsis-" + $props.ellipsis : ""]])
                  },
                  vue.toDisplayString($props.title),
                  3
                  /* TEXT, CLASS */
                )) : vue.createCommentVNode("v-if", true),
                $props.note ? (vue.openBlock(), vue.createElementBlock(
                  "text",
                  {
                    key: 1,
                    class: "uni-list-item__content-note"
                  },
                  vue.toDisplayString($props.note),
                  1
                  /* TEXT */
                )) : vue.createCommentVNode("v-if", true)
              ],
              2
              /* CLASS */
            )
          ], true),
          vue.renderSlot(_ctx.$slots, "footer", {}, () => [
            $props.rightText || $props.showBadge || $props.showSwitch ? (vue.openBlock(), vue.createElementBlock(
              "view",
              {
                key: 0,
                class: vue.normalizeClass(["uni-list-item__extra", { "flex--justify": $props.direction === "column" }])
              },
              [
                $props.rightText ? (vue.openBlock(), vue.createElementBlock(
                  "text",
                  {
                    key: 0,
                    class: "uni-list-item__extra-text"
                  },
                  vue.toDisplayString($props.rightText),
                  1
                  /* TEXT */
                )) : vue.createCommentVNode("v-if", true),
                $props.showBadge ? (vue.openBlock(), vue.createBlock(_component_uni_badge, {
                  key: 1,
                  type: $props.badgeType,
                  text: $props.badgeText,
                  "custom-style": $props.badgeStyle
                }, null, 8, ["type", "text", "custom-style"])) : vue.createCommentVNode("v-if", true),
                $props.showSwitch ? (vue.openBlock(), vue.createElementBlock("switch", {
                  key: 2,
                  disabled: $props.disabled,
                  checked: $props.switchChecked,
                  onChange: _cache[0] || (_cache[0] = (...args) => $options.onSwitchChange && $options.onSwitchChange(...args))
                }, null, 40, ["disabled", "checked"])) : vue.createCommentVNode("v-if", true)
              ],
              2
              /* CLASS */
            )) : vue.createCommentVNode("v-if", true)
          ], true)
        ],
        6
        /* CLASS, STYLE */
      ),
      $props.showArrow || $props.link ? (vue.openBlock(), vue.createBlock(_component_uni_icons, {
        key: 1,
        size: 16,
        class: "uni-icon-wrapper",
        color: "#bbb",
        type: "arrowright"
      })) : vue.createCommentVNode("v-if", true)
    ], 14, ["hover-class"]);
  }
  const __easycom_1$1 = /* @__PURE__ */ _export_sfc(_sfc_main$8, [["render", _sfc_render$7], ["__scopeId", "data-v-c7524739"], ["__file", "E:/NiuNiu/code/YZV25_PDA/uni_modules/uni-list/components/uni-list-item/uni-list-item.vue"]]);
  const _sfc_main$7 = {
    name: "uniList",
    "mp-weixin": {
      options: {
        multipleSlots: false
      }
    },
    props: {
      stackFromEnd: {
        type: Boolean,
        default: false
      },
      enableBackToTop: {
        type: [Boolean, String],
        default: false
      },
      scrollY: {
        type: [Boolean, String],
        default: false
      },
      border: {
        type: Boolean,
        default: true
      },
      renderReverse: {
        type: Boolean,
        default: false
      }
    },
    // provide() {
    // 	return {
    // 		list: this
    // 	};
    // },
    created() {
      this.firstChildAppend = false;
    },
    methods: {
      loadMore(e) {
        this.$emit("scrolltolower");
      },
      scroll(e) {
        this.$emit("scroll", e);
      }
    }
  };
  function _sfc_render$6(_ctx, _cache, $props, $setup, $data, $options) {
    return vue.openBlock(), vue.createElementBlock("view", { class: "uni-list uni-border-top-bottom" }, [
      $props.border ? (vue.openBlock(), vue.createElementBlock("view", {
        key: 0,
        class: "uni-list--border-top"
      })) : vue.createCommentVNode("v-if", true),
      vue.renderSlot(_ctx.$slots, "default", {}, void 0, true),
      $props.border ? (vue.openBlock(), vue.createElementBlock("view", {
        key: 1,
        class: "uni-list--border-bottom"
      })) : vue.createCommentVNode("v-if", true)
    ]);
  }
  const __easycom_2$1 = /* @__PURE__ */ _export_sfc(_sfc_main$7, [["render", _sfc_render$6], ["__scopeId", "data-v-c2f1266a"], ["__file", "E:/NiuNiu/code/YZV25_PDA/uni_modules/uni-list/components/uni-list/uni-list.vue"]]);
  const en = {
    "uni-pagination.prevText": "prev",
    "uni-pagination.nextText": "next",
    "uni-pagination.piecePerPage": "piece/page"
  };
  const es = {
    "uni-pagination.prevText": "anterior",
    "uni-pagination.nextText": "prxima",
    "uni-pagination.piecePerPage": "Art��culo/P��gina"
  };
  const fr = {
    "uni-pagination.prevText": "précédente",
    "uni-pagination.nextText": "suivante",
    "uni-pagination.piecePerPage": "Articles/Pages"
  };
  const zhHans = {
    "uni-pagination.prevText": "上一页",
    "uni-pagination.nextText": "下一页",
    "uni-pagination.piecePerPage": "条/页"
  };
  const zhHant = {
    "uni-pagination.prevText": "上一頁",
    "uni-pagination.nextText": "下一頁",
    "uni-pagination.piecePerPage": "條/頁"
  };
  const messages = {
    en,
    es,
    fr,
    "zh-Hans": zhHans,
    "zh-Hant": zhHant
  };
  const {
    t
  } = initVueI18n(messages);
  const _sfc_main$6 = {
    name: "UniPagination",
    emits: ["update:modelValue", "input", "change", "pageSizeChange"],
    props: {
      value: {
        type: [Number, String],
        default: 1
      },
      modelValue: {
        type: [Number, String],
        default: 1
      },
      prevText: {
        type: String
      },
      nextText: {
        type: String
      },
      piecePerPageText: {
        type: String
      },
      current: {
        type: [Number, String],
        default: 1
      },
      total: {
        // 数据总量
        type: [Number, String],
        default: 0
      },
      pageSize: {
        // 每页数据量
        type: [Number, String],
        default: 10
      },
      showIcon: {
        // 是否以 icon 形式展示按钮
        type: [Boolean, String],
        default: false
      },
      showPageSize: {
        // 是否以 icon 形式展示按钮
        type: [Boolean, String],
        default: false
      },
      pagerCount: {
        type: Number,
        default: 7
      },
      pageSizeRange: {
        type: Array,
        default: () => [20, 50, 100, 500]
      }
    },
    data() {
      return {
        pageSizeIndex: 0,
        currentIndex: 1,
        paperData: [],
        pickerShow: false
      };
    },
    computed: {
      piecePerPage() {
        return this.piecePerPageText || t("uni-pagination.piecePerPage");
      },
      prevPageText() {
        return this.prevText || t("uni-pagination.prevText");
      },
      nextPageText() {
        return this.nextText || t("uni-pagination.nextText");
      },
      maxPage() {
        let maxPage = 1;
        let total = Number(this.total);
        let pageSize = Number(this.pageSize);
        if (total && pageSize) {
          maxPage = Math.ceil(total / pageSize);
        }
        return maxPage;
      },
      paper() {
        const num = this.currentIndex;
        const pagerCount = this.pagerCount;
        const total = this.total;
        const pageSize = this.pageSize;
        let totalArr = [];
        let showPagerArr = [];
        let pagerNum = Math.ceil(total / pageSize);
        for (let i = 0; i < pagerNum; i++) {
          totalArr.push(i + 1);
        }
        showPagerArr.push(1);
        const totalNum = totalArr[totalArr.length - (pagerCount + 1) / 2];
        totalArr.forEach((item, index) => {
          if ((pagerCount + 1) / 2 >= num) {
            if (item < pagerCount + 1 && item > 1) {
              showPagerArr.push(item);
            }
          } else if (num + 2 <= totalNum) {
            if (item > num - (pagerCount + 1) / 2 && item < num + (pagerCount + 1) / 2) {
              showPagerArr.push(item);
            }
          } else {
            if ((item > num - (pagerCount + 1) / 2 || pagerNum - pagerCount < item) && item < totalArr[totalArr.length - 1]) {
              showPagerArr.push(item);
            }
          }
        });
        if (pagerNum > pagerCount) {
          if ((pagerCount + 1) / 2 >= num) {
            showPagerArr[showPagerArr.length - 1] = "...";
          } else if (num + 2 <= totalNum) {
            showPagerArr[1] = "...";
            showPagerArr[showPagerArr.length - 1] = "...";
          } else {
            showPagerArr[1] = "...";
          }
          showPagerArr.push(totalArr[totalArr.length - 1]);
        } else {
          if ((pagerCount + 1) / 2 >= num)
            ;
          else if (num + 2 <= totalNum)
            ;
          else {
            showPagerArr.shift();
            showPagerArr.push(totalArr[totalArr.length - 1]);
          }
        }
        return showPagerArr;
      }
    },
    watch: {
      current: {
        immediate: true,
        handler(val, old) {
          if (val < 1) {
            this.currentIndex = 1;
          } else {
            this.currentIndex = val;
          }
        }
      },
      value: {
        immediate: true,
        handler(val) {
          if (Number(this.current) !== 1)
            return;
          if (val < 1) {
            this.currentIndex = 1;
          } else {
            this.currentIndex = val;
          }
        }
      },
      pageSizeIndex(val) {
        this.$emit("pageSizeChange", this.pageSizeRange[val]);
      }
    },
    methods: {
      pickerChange(e) {
        this.pageSizeIndex = e.detail.value;
        this.pickerClick();
      },
      pickerClick() {
      },
      // 选择标签
      selectPage(e, index) {
        if (parseInt(e)) {
          this.currentIndex = e;
          this.change("current");
        } else {
          let pagerNum = Math.ceil(this.total / this.pageSize);
          if (index <= 1) {
            if (this.currentIndex - 5 > 1) {
              this.currentIndex -= 5;
            } else {
              this.currentIndex = 1;
            }
            return;
          }
          if (index >= 6) {
            if (this.currentIndex + 5 > pagerNum) {
              this.currentIndex = pagerNum;
            } else {
              this.currentIndex += 5;
            }
            return;
          }
        }
      },
      clickLeft() {
        if (Number(this.currentIndex) === 1) {
          return;
        }
        this.currentIndex -= 1;
        this.change("prev");
      },
      clickRight() {
        if (Number(this.currentIndex) >= this.maxPage) {
          return;
        }
        this.currentIndex += 1;
        this.change("next");
      },
      change(e) {
        this.$emit("input", this.currentIndex);
        this.$emit("update:modelValue", this.currentIndex);
        this.$emit("change", {
          type: e,
          current: this.currentIndex
        });
      }
    }
  };
  function _sfc_render$5(_ctx, _cache, $props, $setup, $data, $options) {
    const _component_uni_icons = resolveEasycom(vue.resolveDynamicComponent("uni-icons"), __easycom_0$2);
    return vue.openBlock(), vue.createElementBlock("view", { class: "uni-pagination" }, [
      $props.showPageSize === true || $props.showPageSize === "true" ? (vue.openBlock(), vue.createElementBlock("picker", {
        key: 0,
        class: "select-picker",
        mode: "selector",
        value: $data.pageSizeIndex,
        range: $props.pageSizeRange,
        onChange: _cache[0] || (_cache[0] = (...args) => $options.pickerChange && $options.pickerChange(...args)),
        onCancel: _cache[1] || (_cache[1] = (...args) => $options.pickerClick && $options.pickerClick(...args)),
        onClick: _cache[2] || (_cache[2] = (...args) => $options.pickerClick && $options.pickerClick(...args))
      }, [
        vue.createElementVNode("button", {
          type: "default",
          size: "mini",
          plain: true
        }, [
          vue.createElementVNode(
            "text",
            null,
            vue.toDisplayString($props.pageSizeRange[$data.pageSizeIndex]) + " " + vue.toDisplayString($options.piecePerPage),
            1
            /* TEXT */
          ),
          vue.createVNode(_component_uni_icons, {
            class: "select-picker-icon",
            type: "arrowdown",
            size: "12",
            color: "#999"
          })
        ])
      ], 40, ["value", "range"])) : vue.createCommentVNode("v-if", true),
      vue.createElementVNode(
        "view",
        { class: "uni-pagination__total is-phone-hide" },
        "共 " + vue.toDisplayString($props.total) + " 条",
        1
        /* TEXT */
      ),
      vue.createElementVNode("view", {
        class: vue.normalizeClass(["uni-pagination__btn", $data.currentIndex === 1 ? "uni-pagination--disabled" : "uni-pagination--enabled"]),
        "hover-class": $data.currentIndex === 1 ? "" : "uni-pagination--hover",
        "hover-start-time": 20,
        "hover-stay-time": 70,
        onClick: _cache[3] || (_cache[3] = (...args) => $options.clickLeft && $options.clickLeft(...args))
      }, [
        $props.showIcon === true || $props.showIcon === "true" ? (vue.openBlock(), vue.createBlock(_component_uni_icons, {
          key: 0,
          color: "#666",
          size: "16",
          type: "left"
        })) : (vue.openBlock(), vue.createElementBlock(
          "text",
          {
            key: 1,
            class: "uni-pagination__child-btn"
          },
          vue.toDisplayString($options.prevPageText),
          1
          /* TEXT */
        ))
      ], 10, ["hover-class"]),
      vue.createElementVNode("view", { class: "uni-pagination__num uni-pagination__num-flex-none" }, [
        vue.createElementVNode("view", { class: "uni-pagination__num-current" }, [
          vue.createElementVNode(
            "text",
            { class: "uni-pagination__num-current-text is-pc-hide current-index-text" },
            vue.toDisplayString($data.currentIndex),
            1
            /* TEXT */
          ),
          vue.createElementVNode(
            "text",
            { class: "uni-pagination__num-current-text is-pc-hide" },
            "/" + vue.toDisplayString($options.maxPage || 0),
            1
            /* TEXT */
          ),
          (vue.openBlock(true), vue.createElementBlock(
            vue.Fragment,
            null,
            vue.renderList($options.paper, (item, index) => {
              return vue.openBlock(), vue.createElementBlock("view", {
                key: index,
                class: vue.normalizeClass([{ "page--active": item === $data.currentIndex }, "uni-pagination__num-tag tag--active is-phone-hide"]),
                onClick: ($event) => $options.selectPage(item, index)
              }, [
                vue.createElementVNode(
                  "text",
                  null,
                  vue.toDisplayString(item),
                  1
                  /* TEXT */
                )
              ], 10, ["onClick"]);
            }),
            128
            /* KEYED_FRAGMENT */
          ))
        ])
      ]),
      vue.createElementVNode("view", {
        class: vue.normalizeClass(["uni-pagination__btn", $data.currentIndex >= $options.maxPage ? "uni-pagination--disabled" : "uni-pagination--enabled"]),
        "hover-class": $data.currentIndex === $options.maxPage ? "" : "uni-pagination--hover",
        "hover-start-time": 20,
        "hover-stay-time": 70,
        onClick: _cache[4] || (_cache[4] = (...args) => $options.clickRight && $options.clickRight(...args))
      }, [
        $props.showIcon === true || $props.showIcon === "true" ? (vue.openBlock(), vue.createBlock(_component_uni_icons, {
          key: 0,
          color: "#666",
          size: "16",
          type: "right"
        })) : (vue.openBlock(), vue.createElementBlock(
          "text",
          {
            key: 1,
            class: "uni-pagination__child-btn"
          },
          vue.toDisplayString($options.nextPageText),
          1
          /* TEXT */
        ))
      ], 10, ["hover-class"])
    ]);
  }
  const __easycom_3 = /* @__PURE__ */ _export_sfc(_sfc_main$6, [["render", _sfc_render$5], ["__scopeId", "data-v-88b7506d"], ["__file", "E:/NiuNiu/code/YZV25_PDA/uni_modules/uni-pagination/components/uni-pagination/uni-pagination.vue"]]);
  const _sfc_main$5 = {
    data() {
      return {
        dataList: [],
        totalCount: 0,
        currentPage: 1,
        pageSize: 10,
        currentTheme: "light"
      };
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
        const savedTheme = uni.getStorageSync("appTheme");
        if (savedTheme) {
          this.currentTheme = savedTheme;
        }
      },
      goBack() {
        uni.navigateBack();
      },
      requestData() {
        this.$dbUtils.getCount("dataDB", "data").then((res) => {
          formatAppLog("log", "at pages/index/index2.vue:76", "数据总量:", res);
          if (res && res.length > 0) {
            this.totalCount = res[0].num || 0;
          }
        }).catch((err) => {
          formatAppLog("log", "at pages/index/index2.vue:81", "获取数据总量失败:", err);
        });
        this.$dbUtils.getDataList("dataDB", "data", this.currentPage, 10, "id", "desc").then((res) => {
          formatAppLog("log", "at pages/index/index2.vue:86", "历史数据列表:", res);
          this.dataList = res || [];
        }).catch((err) => {
          formatAppLog("log", "at pages/index/index2.vue:89", "获取历史数据失败:", err);
          this.dataList = [];
        });
      },
      pageChange(e) {
        this.currentPage = e.current;
        this.requestData();
      },
      searchPress(searchValue) {
        this.$dbUtils.selectDataListByLike("dataDB", "data", "scancode", searchValue.value, "id", "desc").then((res) => {
          formatAppLog("log", "at pages/index/index2.vue:100", "搜索结果:", res.length);
          this.currentPage = 1;
          this.totalCount = res.length;
          this.dataList = res || [];
        });
        uni.showToast({
          title: "正在查询：" + searchValue.value,
          icon: "none"
        });
      },
      onClick(item, e) {
        uni.showModal({
          title: "数据",
          content: JSON.stringify(item),
          showCancel: false
        });
      }
    }
  };
  function _sfc_render$4(_ctx, _cache, $props, $setup, $data, $options) {
    const _component_uni_search_bar = resolveEasycom(vue.resolveDynamicComponent("uni-search-bar"), __easycom_0$1);
    const _component_uni_list_item = resolveEasycom(vue.resolveDynamicComponent("uni-list-item"), __easycom_1$1);
    const _component_uni_list = resolveEasycom(vue.resolveDynamicComponent("uni-list"), __easycom_2$1);
    const _component_uni_pagination = resolveEasycom(vue.resolveDynamicComponent("uni-pagination"), __easycom_3);
    return vue.openBlock(), vue.createElementBlock(
      "view",
      {
        class: vue.normalizeClass(["history-container", ["theme-" + $data.currentTheme]])
      },
      [
        vue.createCommentVNode(" 自定义导航栏 "),
        vue.createElementVNode("view", { class: "custom-nav" }, [
          vue.createElementVNode("view", {
            class: "nav-left",
            onClick: _cache[0] || (_cache[0] = (...args) => $options.goBack && $options.goBack(...args))
          }, [
            vue.createElementVNode("view", { class: "nav-icon back-icon" }),
            vue.createElementVNode("text", { class: "nav-text" }, "返回")
          ]),
          vue.createElementVNode("text", { class: "nav-title" }, "历史数据"),
          vue.createElementVNode("view", { class: "nav-right" })
        ]),
        vue.createCommentVNode(" 主内容区域 "),
        vue.createElementVNode("view", { class: "main-content" }, [
          vue.createCommentVNode(" 搜索栏 "),
          vue.createElementVNode("view", { class: "search-wrapper" }, [
            vue.createVNode(_component_uni_search_bar, {
              radius: "5",
              placeholder: "输入要搜索的内容",
              clearButton: "always",
              "cancel-text": "搜索",
              onCancel: $options.searchPress
            }, null, 8, ["onCancel"])
          ]),
          vue.createCommentVNode(" 列表区域 "),
          vue.createElementVNode("scroll-view", {
            class: "list-scroll",
            "scroll-y": ""
          }, [
            vue.createVNode(_component_uni_list, { class: "myList" }, {
              default: vue.withCtx(() => [
                (vue.openBlock(true), vue.createElementBlock(
                  vue.Fragment,
                  null,
                  vue.renderList($data.dataList, (item, index) => {
                    return vue.openBlock(), vue.createBlock(_component_uni_list_item, {
                      class: "myItem",
                      key: index,
                      title: item.scancode,
                      note: item.ctime,
                      ellipsis: 1,
                      clickable: "",
                      onClick: ($event) => $options.onClick(item)
                    }, null, 8, ["title", "note", "onClick"]);
                  }),
                  128
                  /* KEYED_FRAGMENT */
                ))
              ]),
              _: 1
              /* STABLE */
            }),
            $data.dataList.length === 0 ? (vue.openBlock(), vue.createElementBlock("view", {
              key: 0,
              class: "empty-tip"
            }, [
              vue.createElementVNode("text", null, "暂无历史数据")
            ])) : vue.createCommentVNode("v-if", true)
          ]),
          vue.createCommentVNode(" 底部区域 "),
          vue.createElementVNode("view", { class: "bottom-area" }, [
            vue.createVNode(_component_uni_pagination, {
              class: "myPagination",
              current: $data.currentPage,
              total: $data.totalCount,
              "page-size": $data.pageSize,
              onChange: $options.pageChange
            }, null, 8, ["current", "total", "page-size", "onChange"]),
            vue.createElementVNode("view", { class: "btn-view" }, [
              vue.createElementVNode(
                "text",
                { class: "example-info" },
                "当前页：" + vue.toDisplayString($data.currentPage) + "，数据总量：" + vue.toDisplayString($data.totalCount) + "条，每页数据：" + vue.toDisplayString($data.pageSize),
                1
                /* TEXT */
              )
            ])
          ])
        ])
      ],
      2
      /* CLASS */
    );
  }
  const PagesIndexIndex2 = /* @__PURE__ */ _export_sfc(_sfc_main$5, [["render", _sfc_render$4], ["__file", "E:/NiuNiu/code/YZV25_PDA/pages/index/index2.vue"]]);
  function obj2strClass(obj) {
    let classess = "";
    for (let key in obj) {
      const val = obj[key];
      if (val) {
        classess += `${key} `;
      }
    }
    return classess;
  }
  function obj2strStyle(obj) {
    let style = "";
    for (let key in obj) {
      const val = obj[key];
      style += `${key}:${val};`;
    }
    return style;
  }
  const _sfc_main$4 = {
    name: "uni-easyinput",
    emits: [
      "click",
      "iconClick",
      "update:modelValue",
      "input",
      "focus",
      "blur",
      "confirm",
      "clear",
      "eyes",
      "change",
      "keyboardheightchange"
    ],
    model: {
      prop: "modelValue",
      event: "update:modelValue"
    },
    options: {},
    inject: {
      form: {
        from: "uniForm",
        default: null
      },
      formItem: {
        from: "uniFormItem",
        default: null
      }
    },
    props: {
      name: String,
      value: [Number, String],
      modelValue: [Number, String],
      type: {
        type: String,
        default: "text"
      },
      clearable: {
        type: Boolean,
        default: true
      },
      autoHeight: {
        type: Boolean,
        default: false
      },
      placeholder: {
        type: String,
        default: " "
      },
      placeholderStyle: String,
      focus: {
        type: Boolean,
        default: false
      },
      disabled: {
        type: Boolean,
        default: false
      },
      maxlength: {
        type: [Number, String],
        default: 140
      },
      confirmType: {
        type: String,
        default: "done"
      },
      clearSize: {
        type: [Number, String],
        default: 24
      },
      inputBorder: {
        type: Boolean,
        default: true
      },
      prefixIcon: {
        type: String,
        default: ""
      },
      suffixIcon: {
        type: String,
        default: ""
      },
      trim: {
        type: [Boolean, String],
        default: false
      },
      cursorSpacing: {
        type: Number,
        default: 0
      },
      passwordIcon: {
        type: Boolean,
        default: true
      },
      adjustPosition: {
        type: Boolean,
        default: true
      },
      primaryColor: {
        type: String,
        default: "#2979ff"
      },
      styles: {
        type: Object,
        default() {
          return {
            color: "#333",
            backgroundColor: "#fff",
            disableColor: "#F7F6F6",
            borderColor: "#e5e5e5"
          };
        }
      },
      errorMessage: {
        type: [String, Boolean],
        default: ""
      }
    },
    data() {
      return {
        focused: false,
        val: "",
        showMsg: "",
        border: false,
        isFirstBorder: false,
        showClearIcon: false,
        showPassword: false,
        focusShow: false,
        localMsg: "",
        isEnter: false
        // 用于判断当前是否是使用回车操作
      };
    },
    computed: {
      // 输入框内是否有值
      isVal() {
        const val = this.val;
        if (val || val === 0) {
          return true;
        }
        return false;
      },
      msg() {
        return this.localMsg || this.errorMessage;
      },
      // 因为uniapp的input组件的maxlength组件必须要数值，这里转为数值，用户可以传入字符串数值
      inputMaxlength() {
        return Number(this.maxlength);
      },
      // 处理外层样式的style
      boxStyle() {
        return `color:${this.inputBorder && this.msg ? "#e43d33" : this.styles.color};`;
      },
      // input 内容的类和样式处理
      inputContentClass() {
        return obj2strClass({
          "is-input-border": this.inputBorder,
          "is-input-error-border": this.inputBorder && this.msg,
          "is-textarea": this.type === "textarea",
          "is-disabled": this.disabled,
          "is-focused": this.focusShow
        });
      },
      inputContentStyle() {
        const focusColor = this.focusShow ? this.primaryColor : this.styles.borderColor;
        const borderColor = this.inputBorder && this.msg ? "#dd524d" : focusColor;
        return obj2strStyle({
          "border-color": borderColor || "#e5e5e5",
          "background-color": this.disabled ? this.styles.disableColor : this.styles.backgroundColor
        });
      },
      // input右侧样式
      inputStyle() {
        const paddingRight = this.type === "password" || this.clearable || this.prefixIcon ? "" : "10px";
        return obj2strStyle({
          "padding-right": paddingRight,
          "padding-left": this.prefixIcon ? "" : "10px"
        });
      }
    },
    watch: {
      value(newVal) {
        this.val = newVal;
      },
      modelValue(newVal) {
        this.val = newVal;
      },
      focus(newVal) {
        this.$nextTick(() => {
          this.focused = this.focus;
          this.focusShow = this.focus;
        });
      }
    },
    created() {
      this.init();
      if (this.form && this.formItem) {
        this.$watch("formItem.errMsg", (newVal) => {
          this.localMsg = newVal;
        });
      }
    },
    mounted() {
      this.$nextTick(() => {
        this.focused = this.focus;
        this.focusShow = this.focus;
      });
    },
    methods: {
      /**
       * 初始化变量值
       */
      init() {
        if (this.value || this.value === 0) {
          this.val = this.value;
        } else if (this.modelValue || this.modelValue === 0 || this.modelValue === "") {
          this.val = this.modelValue;
        } else {
          this.val = null;
        }
      },
      /**
       * 点击图标时触发
       * @param {Object} type
       */
      onClickIcon(type) {
        this.$emit("iconClick", type);
      },
      /**
       * 显示隐藏内容，密码框时生效
       */
      onEyes() {
        this.showPassword = !this.showPassword;
        this.$emit("eyes", this.showPassword);
      },
      /**
       * 输入时触发
       * @param {Object} event
       */
      onInput(event) {
        let value = event.detail.value;
        if (this.trim) {
          if (typeof this.trim === "boolean" && this.trim) {
            value = this.trimStr(value);
          }
          if (typeof this.trim === "string") {
            value = this.trimStr(value, this.trim);
          }
        }
        if (this.errMsg)
          this.errMsg = "";
        this.val = value;
        this.$emit("input", value);
        this.$emit("update:modelValue", value);
      },
      /**
       * 外部调用方法
       * 获取焦点时触发
       * @param {Object} event
       */
      onFocus() {
        this.$nextTick(() => {
          this.focused = true;
        });
        this.$emit("focus", null);
      },
      _Focus(event) {
        this.focusShow = true;
        this.$emit("focus", event);
      },
      /**
       * 外部调用方法
       * 失去焦点时触发
       * @param {Object} event
       */
      onBlur() {
        this.focused = false;
        this.$emit("blur", null);
      },
      _Blur(event) {
        event.detail.value;
        this.focusShow = false;
        this.$emit("blur", event);
        if (this.isEnter === false) {
          this.$emit("change", this.val);
        }
        if (this.form && this.formItem) {
          const { validateTrigger } = this.form;
          if (validateTrigger === "blur") {
            this.formItem.onFieldChange();
          }
        }
      },
      /**
       * 按下键盘的发送键
       * @param {Object} e
       */
      onConfirm(e) {
        this.$emit("confirm", this.val);
        this.isEnter = true;
        this.$emit("change", this.val);
        this.$nextTick(() => {
          this.isEnter = false;
        });
      },
      /**
       * 清理内容
       * @param {Object} event
       */
      onClear(event) {
        this.val = "";
        this.$emit("input", "");
        this.$emit("update:modelValue", "");
        this.$emit("clear");
      },
      /**
       * 键盘高度发生变化的时候触发此事件
       * 兼容性：微信小程序2.7.0+、App 3.1.0+
       * @param {Object} event
       */
      onkeyboardheightchange(event) {
        this.$emit("keyboardheightchange", event);
      },
      /**
       * 去除空格
       */
      trimStr(str2, pos = "both") {
        if (pos === "both") {
          return str2.trim();
        } else if (pos === "left") {
          return str2.trimLeft();
        } else if (pos === "right") {
          return str2.trimRight();
        } else if (pos === "start") {
          return str2.trimStart();
        } else if (pos === "end") {
          return str2.trimEnd();
        } else if (pos === "all") {
          return str2.replace(/\s+/g, "");
        } else if (pos === "none") {
          return str2;
        }
        return str2;
      }
    }
  };
  function _sfc_render$3(_ctx, _cache, $props, $setup, $data, $options) {
    const _component_uni_icons = resolveEasycom(vue.resolveDynamicComponent("uni-icons"), __easycom_0$2);
    return vue.openBlock(), vue.createElementBlock(
      "view",
      {
        class: vue.normalizeClass(["uni-easyinput", { "uni-easyinput-error": $options.msg }]),
        style: vue.normalizeStyle($options.boxStyle)
      },
      [
        vue.createElementVNode(
          "view",
          {
            class: vue.normalizeClass(["uni-easyinput__content", $options.inputContentClass]),
            style: vue.normalizeStyle($options.inputContentStyle)
          },
          [
            $props.prefixIcon ? (vue.openBlock(), vue.createBlock(_component_uni_icons, {
              key: 0,
              class: "content-clear-icon",
              type: $props.prefixIcon,
              color: "#c0c4cc",
              onClick: _cache[0] || (_cache[0] = ($event) => $options.onClickIcon("prefix")),
              size: "22"
            }, null, 8, ["type"])) : vue.createCommentVNode("v-if", true),
            vue.renderSlot(_ctx.$slots, "left", {}, void 0, true),
            $props.type === "textarea" ? (vue.openBlock(), vue.createElementBlock("textarea", {
              key: 1,
              class: vue.normalizeClass(["uni-easyinput__content-textarea", { "input-padding": $props.inputBorder }]),
              name: $props.name,
              value: $data.val,
              placeholder: $props.placeholder,
              placeholderStyle: $props.placeholderStyle,
              disabled: $props.disabled,
              "placeholder-class": "uni-easyinput__placeholder-class",
              maxlength: $options.inputMaxlength,
              focus: $data.focused,
              autoHeight: $props.autoHeight,
              "cursor-spacing": $props.cursorSpacing,
              "adjust-position": $props.adjustPosition,
              onInput: _cache[1] || (_cache[1] = (...args) => $options.onInput && $options.onInput(...args)),
              onBlur: _cache[2] || (_cache[2] = (...args) => $options._Blur && $options._Blur(...args)),
              onFocus: _cache[3] || (_cache[3] = (...args) => $options._Focus && $options._Focus(...args)),
              onConfirm: _cache[4] || (_cache[4] = (...args) => $options.onConfirm && $options.onConfirm(...args)),
              onKeyboardheightchange: _cache[5] || (_cache[5] = (...args) => $options.onkeyboardheightchange && $options.onkeyboardheightchange(...args))
            }, null, 42, ["name", "value", "placeholder", "placeholderStyle", "disabled", "maxlength", "focus", "autoHeight", "cursor-spacing", "adjust-position"])) : (vue.openBlock(), vue.createElementBlock("input", {
              key: 2,
              type: $props.type === "password" ? "text" : $props.type,
              class: "uni-easyinput__content-input",
              style: vue.normalizeStyle($options.inputStyle),
              name: $props.name,
              value: $data.val,
              password: !$data.showPassword && $props.type === "password",
              placeholder: $props.placeholder,
              placeholderStyle: $props.placeholderStyle,
              "placeholder-class": "uni-easyinput__placeholder-class",
              disabled: $props.disabled,
              maxlength: $options.inputMaxlength,
              focus: $data.focused,
              confirmType: $props.confirmType,
              "cursor-spacing": $props.cursorSpacing,
              "adjust-position": $props.adjustPosition,
              onFocus: _cache[6] || (_cache[6] = (...args) => $options._Focus && $options._Focus(...args)),
              onBlur: _cache[7] || (_cache[7] = (...args) => $options._Blur && $options._Blur(...args)),
              onInput: _cache[8] || (_cache[8] = (...args) => $options.onInput && $options.onInput(...args)),
              onConfirm: _cache[9] || (_cache[9] = (...args) => $options.onConfirm && $options.onConfirm(...args)),
              onKeyboardheightchange: _cache[10] || (_cache[10] = (...args) => $options.onkeyboardheightchange && $options.onkeyboardheightchange(...args))
            }, null, 44, ["type", "name", "value", "password", "placeholder", "placeholderStyle", "disabled", "maxlength", "focus", "confirmType", "cursor-spacing", "adjust-position"])),
            $props.type === "password" && $props.passwordIcon ? (vue.openBlock(), vue.createElementBlock(
              vue.Fragment,
              { key: 3 },
              [
                vue.createCommentVNode(" 开启密码时显示小眼睛 "),
                $options.isVal ? (vue.openBlock(), vue.createBlock(_component_uni_icons, {
                  key: 0,
                  class: vue.normalizeClass(["content-clear-icon", { "is-textarea-icon": $props.type === "textarea" }]),
                  type: $data.showPassword ? "eye-slash-filled" : "eye-filled",
                  size: 22,
                  color: $data.focusShow ? $props.primaryColor : "#c0c4cc",
                  onClick: $options.onEyes
                }, null, 8, ["class", "type", "color", "onClick"])) : vue.createCommentVNode("v-if", true)
              ],
              64
              /* STABLE_FRAGMENT */
            )) : vue.createCommentVNode("v-if", true),
            $props.suffixIcon ? (vue.openBlock(), vue.createElementBlock(
              vue.Fragment,
              { key: 4 },
              [
                $props.suffixIcon ? (vue.openBlock(), vue.createBlock(_component_uni_icons, {
                  key: 0,
                  class: "content-clear-icon",
                  type: $props.suffixIcon,
                  color: "#c0c4cc",
                  onClick: _cache[11] || (_cache[11] = ($event) => $options.onClickIcon("suffix")),
                  size: "22"
                }, null, 8, ["type"])) : vue.createCommentVNode("v-if", true)
              ],
              64
              /* STABLE_FRAGMENT */
            )) : (vue.openBlock(), vue.createElementBlock(
              vue.Fragment,
              { key: 5 },
              [
                $props.clearable && $options.isVal && !$props.disabled && $props.type !== "textarea" ? (vue.openBlock(), vue.createBlock(_component_uni_icons, {
                  key: 0,
                  class: vue.normalizeClass(["content-clear-icon", { "is-textarea-icon": $props.type === "textarea" }]),
                  type: "clear",
                  size: $props.clearSize,
                  color: $options.msg ? "#dd524d" : $data.focusShow ? $props.primaryColor : "#c0c4cc",
                  onClick: $options.onClear
                }, null, 8, ["class", "size", "color", "onClick"])) : vue.createCommentVNode("v-if", true)
              ],
              64
              /* STABLE_FRAGMENT */
            )),
            vue.renderSlot(_ctx.$slots, "right", {}, void 0, true)
          ],
          6
          /* CLASS, STYLE */
        )
      ],
      6
      /* CLASS, STYLE */
    );
  }
  const __easycom_0 = /* @__PURE__ */ _export_sfc(_sfc_main$4, [["render", _sfc_render$3], ["__scopeId", "data-v-09fd5285"], ["__file", "E:/NiuNiu/code/YZV25_PDA/uni_modules/uni-easyinput/components/uni-easyinput/uni-easyinput.vue"]]);
  const _sfc_main$3 = {
    name: "uniFormsItem",
    options: {
      virtualHost: true
    },
    provide() {
      return {
        uniFormItem: this
      };
    },
    inject: {
      form: {
        from: "uniForm",
        default: null
      }
    },
    props: {
      // 表单校验规则
      rules: {
        type: Array,
        default() {
          return null;
        }
      },
      // 表单域的属性名，在使用校验规则时必填
      name: {
        type: [String, Array],
        default: ""
      },
      required: {
        type: Boolean,
        default: false
      },
      label: {
        type: String,
        default: ""
      },
      // label的宽度
      labelWidth: {
        type: [String, Number],
        default: ""
      },
      // label 居中方式，默认 left 取值 left/center/right
      labelAlign: {
        type: String,
        default: ""
      },
      // 强制显示错误信息
      errorMessage: {
        type: [String, Boolean],
        default: ""
      },
      // 1.4.0 弃用，统一使用 form 的校验时机
      // validateTrigger: {
      // 	type: String,
      // 	default: ''
      // },
      // 1.4.0 弃用，统一使用 form 的label 位置
      // labelPosition: {
      // 	type: String,
      // 	default: ''
      // },
      // 1.4.0 以下属性已经废弃，请使用  #label 插槽代替
      leftIcon: String,
      iconColor: {
        type: String,
        default: "#606266"
      }
    },
    data() {
      return {
        errMsg: "",
        userRules: null,
        localLabelAlign: "left",
        localLabelWidth: "70px",
        localLabelPos: "left",
        border: false,
        isFirstBorder: false
      };
    },
    computed: {
      // 处理错误信息
      msg() {
        return this.errorMessage || this.errMsg;
      }
    },
    watch: {
      // 规则发生变化通知子组件更新
      "form.formRules"(val) {
        this.init();
      },
      "form.labelWidth"(val) {
        this.localLabelWidth = this._labelWidthUnit(val);
      },
      "form.labelPosition"(val) {
        this.localLabelPos = this._labelPosition();
      },
      "form.labelAlign"(val) {
      }
    },
    created() {
      this.init(true);
      if (this.name && this.form) {
        this.$watch(
          () => {
            const val = this.form._getDataValue(this.name, this.form.localData);
            return val;
          },
          (value, oldVal) => {
            const isEqual2 = this.form._isEqual(value, oldVal);
            if (!isEqual2) {
              const val = this.itemSetValue(value);
              this.onFieldChange(val, false);
            }
          },
          {
            immediate: false
          }
        );
      }
    },
    unmounted() {
      this.__isUnmounted = true;
      this.unInit();
    },
    methods: {
      /**
       * 外部调用方法
       * 设置规则 ，主要用于小程序自定义检验规则
       * @param {Array} rules 规则源数据
       */
      setRules(rules = null) {
        this.userRules = rules;
        this.init(false);
      },
      // 兼容老版本表单组件
      setValue() {
      },
      /**
       * 外部调用方法
       * 校验数据
       * @param {any} value 需要校验的数据
       * @param {boolean} 是否立即校验
       * @return {Array|null} 校验内容
       */
      async onFieldChange(value, formtrigger = true) {
        const {
          formData,
          localData,
          errShowType,
          validateCheck,
          validateTrigger,
          _isRequiredField,
          _realName
        } = this.form;
        const name = _realName(this.name);
        if (!value) {
          value = this.form.formData[name];
        }
        const ruleLen = this.itemRules.rules && this.itemRules.rules.length;
        if (!this.validator || !ruleLen || ruleLen === 0)
          return;
        const isRequiredField2 = _isRequiredField(this.itemRules.rules || []);
        let result = null;
        if (validateTrigger === "bind" || formtrigger) {
          result = await this.validator.validateUpdate(
            {
              [name]: value
            },
            formData
          );
          if (!isRequiredField2 && (value === void 0 || value === "")) {
            result = null;
          }
          if (result && result.errorMessage) {
            if (errShowType === "undertext") {
              this.errMsg = !result ? "" : result.errorMessage;
            }
            if (errShowType === "toast") {
              uni.showToast({
                title: result.errorMessage || "校验错误",
                icon: "none"
              });
            }
            if (errShowType === "modal") {
              uni.showModal({
                title: "提示",
                content: result.errorMessage || "校验错误"
              });
            }
          } else {
            this.errMsg = "";
          }
          validateCheck(result ? result : null);
        } else {
          this.errMsg = "";
        }
        return result ? result : null;
      },
      /**
       * 初始组件数据
       */
      init(type = false) {
        const {
          validator,
          formRules,
          childrens,
          formData,
          localData,
          _realName,
          labelWidth,
          _getDataValue,
          _setDataValue
        } = this.form || {};
        this.localLabelAlign = this._justifyContent();
        this.localLabelWidth = this._labelWidthUnit(labelWidth);
        this.localLabelPos = this._labelPosition();
        this.form && type && childrens.push(this);
        if (!validator || !formRules)
          return;
        if (!this.form.isFirstBorder) {
          this.form.isFirstBorder = true;
          this.isFirstBorder = true;
        }
        if (this.group) {
          if (!this.group.isFirstBorder) {
            this.group.isFirstBorder = true;
            this.isFirstBorder = true;
          }
        }
        this.border = this.form.border;
        const name = _realName(this.name);
        const itemRule = this.userRules || this.rules;
        if (typeof formRules === "object" && itemRule) {
          formRules[name] = {
            rules: itemRule
          };
          validator.updateSchema(formRules);
        }
        const itemRules = formRules[name] || {};
        this.itemRules = itemRules;
        this.validator = validator;
        this.itemSetValue(_getDataValue(this.name, localData));
      },
      unInit() {
        if (this.form) {
          const {
            childrens,
            formData,
            _realName
          } = this.form;
          childrens.forEach((item, index) => {
            if (item === this) {
              this.form.childrens.splice(index, 1);
              delete formData[_realName(item.name)];
            }
          });
        }
      },
      // 设置item 的值
      itemSetValue(value) {
        const name = this.form._realName(this.name);
        const rules = this.itemRules.rules || [];
        const val = this.form._getValue(name, value, rules);
        this.form._setDataValue(name, this.form.formData, val);
        return val;
      },
      /**
       * 移除该表单项的校验结果
       */
      clearValidate() {
        this.errMsg = "";
      },
      // 是否显示星号
      _isRequired() {
        return this.required;
      },
      // 处理对齐方式
      _justifyContent() {
        if (this.form) {
          const {
            labelAlign
          } = this.form;
          let labelAli = this.labelAlign ? this.labelAlign : labelAlign;
          if (labelAli === "left")
            return "flex-start";
          if (labelAli === "center")
            return "center";
          if (labelAli === "right")
            return "flex-end";
        }
        return "flex-start";
      },
      // 处理 label宽度单位 ,继承父元素的值
      _labelWidthUnit(labelWidth) {
        return this.num2px(this.labelWidth ? this.labelWidth : labelWidth || (this.label ? 70 : "auto"));
      },
      // 处理 label 位置
      _labelPosition() {
        if (this.form)
          return this.form.labelPosition || "left";
        return "left";
      },
      /**
       * 触发时机
       * @param {Object} rule 当前规则内时机
       * @param {Object} itemRlue 当前组件时机
       * @param {Object} parentRule 父组件时机
       */
      isTrigger(rule, itemRlue, parentRule) {
        if (rule === "submit" || !rule) {
          if (rule === void 0) {
            if (itemRlue !== "bind") {
              if (!itemRlue) {
                return parentRule === "" ? "bind" : "submit";
              }
              return "submit";
            }
            return "bind";
          }
          return "submit";
        }
        return "bind";
      },
      num2px(num) {
        if (typeof num === "number") {
          return `${num}px`;
        }
        return num;
      }
    }
  };
  function _sfc_render$2(_ctx, _cache, $props, $setup, $data, $options) {
    return vue.openBlock(), vue.createElementBlock(
      "view",
      {
        class: vue.normalizeClass(["uni-forms-item", ["is-direction-" + $data.localLabelPos, $data.border ? "uni-forms-item--border" : "", $data.border && $data.isFirstBorder ? "is-first-border" : ""]])
      },
      [
        vue.renderSlot(_ctx.$slots, "label", {}, () => [
          vue.createElementVNode(
            "view",
            {
              class: vue.normalizeClass(["uni-forms-item__label", { "no-label": !$props.label && !$props.required }]),
              style: vue.normalizeStyle({ width: $data.localLabelWidth, justifyContent: $data.localLabelAlign })
            },
            [
              $props.required ? (vue.openBlock(), vue.createElementBlock("text", {
                key: 0,
                class: "is-required"
              }, "*")) : vue.createCommentVNode("v-if", true),
              vue.createElementVNode(
                "text",
                null,
                vue.toDisplayString($props.label),
                1
                /* TEXT */
              )
            ],
            6
            /* CLASS, STYLE */
          )
        ], true),
        vue.createElementVNode("view", { class: "uni-forms-item__content" }, [
          vue.renderSlot(_ctx.$slots, "default", {}, void 0, true),
          vue.createElementVNode(
            "view",
            {
              class: vue.normalizeClass(["uni-forms-item__error", { "msg--active": $options.msg }])
            },
            [
              vue.createElementVNode(
                "text",
                null,
                vue.toDisplayString($options.msg),
                1
                /* TEXT */
              )
            ],
            2
            /* CLASS */
          )
        ])
      ],
      2
      /* CLASS */
    );
  }
  const __easycom_1 = /* @__PURE__ */ _export_sfc(_sfc_main$3, [["render", _sfc_render$2], ["__scopeId", "data-v-462874dd"], ["__file", "E:/NiuNiu/code/YZV25_PDA/uni_modules/uni-forms/components/uni-forms-item/uni-forms-item.vue"]]);
  var pattern = {
    email: /^\S+?@\S+?\.\S+?$/,
    idcard: /^[1-9]\d{5}(18|19|([23]\d))\d{2}((0[1-9])|(10|11|12))(([0-2][1-9])|10|20|30|31)\d{3}[0-9Xx]$/,
    url: new RegExp(
      "^(?!mailto:)(?:(?:http|https|ftp)://|//)(?:\\S+(?::\\S*)?@)?(?:(?:(?:[1-9]\\d?|1\\d\\d|2[01]\\d|22[0-3])(?:\\.(?:1?\\d{1,2}|2[0-4]\\d|25[0-5])){2}(?:\\.(?:[0-9]\\d?|1\\d\\d|2[0-4]\\d|25[0-4]))|(?:(?:[a-z\\u00a1-\\uffff0-9]+-*)*[a-z\\u00a1-\\uffff0-9]+)(?:\\.(?:[a-z\\u00a1-\\uffff0-9]+-*)*[a-z\\u00a1-\\uffff0-9]+)*(?:\\.(?:[a-z\\u00a1-\\uffff]{2,})))|localhost)(?::\\d{2,5})?(?:(/|\\?|#)[^\\s]*)?$",
      "i"
    )
  };
  const FORMAT_MAPPING = {
    "int": "integer",
    "bool": "boolean",
    "double": "number",
    "long": "number",
    "password": "string"
    // "fileurls": 'array'
  };
  function formatMessage(args, resources = "") {
    var defaultMessage = ["label"];
    defaultMessage.forEach((item) => {
      if (args[item] === void 0) {
        args[item] = "";
      }
    });
    let str2 = resources;
    for (let key in args) {
      let reg = new RegExp("{" + key + "}");
      str2 = str2.replace(reg, args[key]);
    }
    return str2;
  }
  function isEmptyValue(value, type) {
    if (value === void 0 || value === null) {
      return true;
    }
    if (typeof value === "string" && !value) {
      return true;
    }
    if (Array.isArray(value) && !value.length) {
      return true;
    }
    if (type === "object" && !Object.keys(value).length) {
      return true;
    }
    return false;
  }
  const types = {
    integer(value) {
      return types.number(value) && parseInt(value, 10) === value;
    },
    string(value) {
      return typeof value === "string";
    },
    number(value) {
      if (isNaN(value)) {
        return false;
      }
      return typeof value === "number";
    },
    "boolean": function(value) {
      return typeof value === "boolean";
    },
    "float": function(value) {
      return types.number(value) && !types.integer(value);
    },
    array(value) {
      return Array.isArray(value);
    },
    object(value) {
      return typeof value === "object" && !types.array(value);
    },
    date(value) {
      return value instanceof Date;
    },
    timestamp(value) {
      if (!this.integer(value) || Math.abs(value).toString().length > 16) {
        return false;
      }
      return true;
    },
    file(value) {
      return typeof value.url === "string";
    },
    email(value) {
      return typeof value === "string" && !!value.match(pattern.email) && value.length < 255;
    },
    url(value) {
      return typeof value === "string" && !!value.match(pattern.url);
    },
    pattern(reg, value) {
      try {
        return new RegExp(reg).test(value);
      } catch (e) {
        return false;
      }
    },
    method(value) {
      return typeof value === "function";
    },
    idcard(value) {
      return typeof value === "string" && !!value.match(pattern.idcard);
    },
    "url-https"(value) {
      return this.url(value) && value.startsWith("https://");
    },
    "url-scheme"(value) {
      return value.startsWith("://");
    },
    "url-web"(value) {
      return false;
    }
  };
  class RuleValidator {
    constructor(message) {
      this._message = message;
    }
    async validateRule(fieldKey, fieldValue, value, data, allData) {
      var result = null;
      let rules = fieldValue.rules;
      let hasRequired = rules.findIndex((item) => {
        return item.required;
      });
      if (hasRequired < 0) {
        if (value === null || value === void 0) {
          return result;
        }
        if (typeof value === "string" && !value.length) {
          return result;
        }
      }
      var message = this._message;
      if (rules === void 0) {
        return message["default"];
      }
      for (var i = 0; i < rules.length; i++) {
        let rule = rules[i];
        let vt = this._getValidateType(rule);
        Object.assign(rule, {
          label: fieldValue.label || `["${fieldKey}"]`
        });
        if (RuleValidatorHelper[vt]) {
          result = RuleValidatorHelper[vt](rule, value, message);
          if (result != null) {
            break;
          }
        }
        if (rule.validateExpr) {
          let now = Date.now();
          let resultExpr = rule.validateExpr(value, allData, now);
          if (resultExpr === false) {
            result = this._getMessage(rule, rule.errorMessage || this._message["default"]);
            break;
          }
        }
        if (rule.validateFunction) {
          result = await this.validateFunction(rule, value, data, allData, vt);
          if (result !== null) {
            break;
          }
        }
      }
      if (result !== null) {
        result = message.TAG + result;
      }
      return result;
    }
    async validateFunction(rule, value, data, allData, vt) {
      let result = null;
      try {
        let callbackMessage = null;
        const res = await rule.validateFunction(rule, value, allData || data, (message) => {
          callbackMessage = message;
        });
        if (callbackMessage || typeof res === "string" && res || res === false) {
          result = this._getMessage(rule, callbackMessage || res, vt);
        }
      } catch (e) {
        result = this._getMessage(rule, e.message, vt);
      }
      return result;
    }
    _getMessage(rule, message, vt) {
      return formatMessage(rule, message || rule.errorMessage || this._message[vt] || message["default"]);
    }
    _getValidateType(rule) {
      var result = "";
      if (rule.required) {
        result = "required";
      } else if (rule.format) {
        result = "format";
      } else if (rule.arrayType) {
        result = "arrayTypeFormat";
      } else if (rule.range) {
        result = "range";
      } else if (rule.maximum !== void 0 || rule.minimum !== void 0) {
        result = "rangeNumber";
      } else if (rule.maxLength !== void 0 || rule.minLength !== void 0) {
        result = "rangeLength";
      } else if (rule.pattern) {
        result = "pattern";
      } else if (rule.validateFunction) {
        result = "validateFunction";
      }
      return result;
    }
  }
  const RuleValidatorHelper = {
    required(rule, value, message) {
      if (rule.required && isEmptyValue(value, rule.format || typeof value)) {
        return formatMessage(rule, rule.errorMessage || message.required);
      }
      return null;
    },
    range(rule, value, message) {
      const {
        range,
        errorMessage
      } = rule;
      let list = new Array(range.length);
      for (let i = 0; i < range.length; i++) {
        const item = range[i];
        if (types.object(item) && item.value !== void 0) {
          list[i] = item.value;
        } else {
          list[i] = item;
        }
      }
      let result = false;
      if (Array.isArray(value)) {
        result = new Set(value.concat(list)).size === list.length;
      } else {
        if (list.indexOf(value) > -1) {
          result = true;
        }
      }
      if (!result) {
        return formatMessage(rule, errorMessage || message["enum"]);
      }
      return null;
    },
    rangeNumber(rule, value, message) {
      if (!types.number(value)) {
        return formatMessage(rule, rule.errorMessage || message.pattern.mismatch);
      }
      let {
        minimum,
        maximum,
        exclusiveMinimum,
        exclusiveMaximum
      } = rule;
      let min = exclusiveMinimum ? value <= minimum : value < minimum;
      let max = exclusiveMaximum ? value >= maximum : value > maximum;
      if (minimum !== void 0 && min) {
        return formatMessage(rule, rule.errorMessage || message["number"][exclusiveMinimum ? "exclusiveMinimum" : "minimum"]);
      } else if (maximum !== void 0 && max) {
        return formatMessage(rule, rule.errorMessage || message["number"][exclusiveMaximum ? "exclusiveMaximum" : "maximum"]);
      } else if (minimum !== void 0 && maximum !== void 0 && (min || max)) {
        return formatMessage(rule, rule.errorMessage || message["number"].range);
      }
      return null;
    },
    rangeLength(rule, value, message) {
      if (!types.string(value) && !types.array(value)) {
        return formatMessage(rule, rule.errorMessage || message.pattern.mismatch);
      }
      let min = rule.minLength;
      let max = rule.maxLength;
      let val = value.length;
      if (min !== void 0 && val < min) {
        return formatMessage(rule, rule.errorMessage || message["length"].minLength);
      } else if (max !== void 0 && val > max) {
        return formatMessage(rule, rule.errorMessage || message["length"].maxLength);
      } else if (min !== void 0 && max !== void 0 && (val < min || val > max)) {
        return formatMessage(rule, rule.errorMessage || message["length"].range);
      }
      return null;
    },
    pattern(rule, value, message) {
      if (!types["pattern"](rule.pattern, value)) {
        return formatMessage(rule, rule.errorMessage || message.pattern.mismatch);
      }
      return null;
    },
    format(rule, value, message) {
      var customTypes = Object.keys(types);
      var format = FORMAT_MAPPING[rule.format] ? FORMAT_MAPPING[rule.format] : rule.format || rule.arrayType;
      if (customTypes.indexOf(format) > -1) {
        if (!types[format](value)) {
          return formatMessage(rule, rule.errorMessage || message.typeError);
        }
      }
      return null;
    },
    arrayTypeFormat(rule, value, message) {
      if (!Array.isArray(value)) {
        return formatMessage(rule, rule.errorMessage || message.typeError);
      }
      for (let i = 0; i < value.length; i++) {
        const element = value[i];
        let formatResult = this.format(rule, element, message);
        if (formatResult !== null) {
          return formatResult;
        }
      }
      return null;
    }
  };
  class SchemaValidator extends RuleValidator {
    constructor(schema, options) {
      super(SchemaValidator.message);
      this._schema = schema;
      this._options = options || null;
    }
    updateSchema(schema) {
      this._schema = schema;
    }
    async validate(data, allData) {
      let result = this._checkFieldInSchema(data);
      if (!result) {
        result = await this.invokeValidate(data, false, allData);
      }
      return result.length ? result[0] : null;
    }
    async validateAll(data, allData) {
      let result = this._checkFieldInSchema(data);
      if (!result) {
        result = await this.invokeValidate(data, true, allData);
      }
      return result;
    }
    async validateUpdate(data, allData) {
      let result = this._checkFieldInSchema(data);
      if (!result) {
        result = await this.invokeValidateUpdate(data, false, allData);
      }
      return result.length ? result[0] : null;
    }
    async invokeValidate(data, all, allData) {
      let result = [];
      let schema = this._schema;
      for (let key in schema) {
        let value = schema[key];
        let errorMessage = await this.validateRule(key, value, data[key], data, allData);
        if (errorMessage != null) {
          result.push({
            key,
            errorMessage
          });
          if (!all)
            break;
        }
      }
      return result;
    }
    async invokeValidateUpdate(data, all, allData) {
      let result = [];
      for (let key in data) {
        let errorMessage = await this.validateRule(key, this._schema[key], data[key], data, allData);
        if (errorMessage != null) {
          result.push({
            key,
            errorMessage
          });
          if (!all)
            break;
        }
      }
      return result;
    }
    _checkFieldInSchema(data) {
      var keys = Object.keys(data);
      var keys2 = Object.keys(this._schema);
      if (new Set(keys.concat(keys2)).size === keys2.length) {
        return "";
      }
      var noExistFields = keys.filter((key) => {
        return keys2.indexOf(key) < 0;
      });
      var errorMessage = formatMessage({
        field: JSON.stringify(noExistFields)
      }, SchemaValidator.message.TAG + SchemaValidator.message["defaultInvalid"]);
      return [{
        key: "invalid",
        errorMessage
      }];
    }
  }
  function Message() {
    return {
      TAG: "",
      default: "验证错误",
      defaultInvalid: "提交的字段{field}在数据库中并不存在",
      validateFunction: "验证无效",
      required: "{label}必填",
      "enum": "{label}超出范围",
      timestamp: "{label}格式无效",
      whitespace: "{label}不能为空",
      typeError: "{label}类型无效",
      date: {
        format: "{label}日期{value}格式无效",
        parse: "{label}日期无法解析,{value}无效",
        invalid: "{label}日期{value}无效"
      },
      length: {
        minLength: "{label}长度不能少于{minLength}",
        maxLength: "{label}长度不能超过{maxLength}",
        range: "{label}必须介于{minLength}和{maxLength}之间"
      },
      number: {
        minimum: "{label}不能小于{minimum}",
        maximum: "{label}不能大于{maximum}",
        exclusiveMinimum: "{label}不能小于等于{minimum}",
        exclusiveMaximum: "{label}不能大于等于{maximum}",
        range: "{label}必须介于{minimum}and{maximum}之间"
      },
      pattern: {
        mismatch: "{label}格式不匹配"
      }
    };
  }
  SchemaValidator.message = new Message();
  const deepCopy = (val) => {
    return JSON.parse(JSON.stringify(val));
  };
  const typeFilter = (format) => {
    return format === "int" || format === "double" || format === "number" || format === "timestamp";
  };
  const getValue = (key, value, rules) => {
    const isRuleNumType = rules.find((val) => val.format && typeFilter(val.format));
    const isRuleBoolType = rules.find((val) => val.format && val.format === "boolean" || val.format === "bool");
    if (!!isRuleNumType) {
      if (!value && value !== 0) {
        value = null;
      } else {
        value = isNumber(Number(value)) ? Number(value) : value;
      }
    }
    if (!!isRuleBoolType) {
      value = isBoolean(value) ? value : false;
    }
    return value;
  };
  const setDataValue = (field, formdata, value) => {
    formdata[field] = value;
    return value || "";
  };
  const getDataValue = (field, data) => {
    return objGet(data, field);
  };
  const realName = (name, data = {}) => {
    const base_name = _basePath(name);
    if (typeof base_name === "object" && Array.isArray(base_name) && base_name.length > 1) {
      const realname = base_name.reduce((a, b) => a += `#${b}`, "_formdata_");
      return realname;
    }
    return base_name[0] || name;
  };
  const isRealName = (name) => {
    const reg = /^_formdata_#*/;
    return reg.test(name);
  };
  const rawData = (object = {}, name) => {
    let newData = JSON.parse(JSON.stringify(object));
    let formData = {};
    for (let i in newData) {
      let path = name2arr(i);
      objSet(formData, path, newData[i]);
    }
    return formData;
  };
  const name2arr = (name) => {
    let field = name.replace("_formdata_#", "");
    field = field.split("#").map((v) => isNumber(v) ? Number(v) : v);
    return field;
  };
  const objSet = (object, path, value) => {
    if (typeof object !== "object")
      return object;
    _basePath(path).reduce((o, k, i, _) => {
      if (i === _.length - 1) {
        o[k] = value;
        return null;
      } else if (k in o) {
        return o[k];
      } else {
        o[k] = /^[0-9]{1,}$/.test(_[i + 1]) ? [] : {};
        return o[k];
      }
    }, object);
    return object;
  };
  function _basePath(path) {
    if (Array.isArray(path))
      return path;
    return path.replace(/\[/g, ".").replace(/\]/g, "").split(".");
  }
  const objGet = (object, path, defaultVal = "undefined") => {
    let newPath = _basePath(path);
    let val = newPath.reduce((o, k) => {
      return (o || {})[k];
    }, object);
    return !val || val !== void 0 ? val : defaultVal;
  };
  const isNumber = (num) => {
    return !isNaN(Number(num));
  };
  const isBoolean = (bool) => {
    return typeof bool === "boolean";
  };
  const isRequiredField = (rules) => {
    let isNoField = false;
    for (let i = 0; i < rules.length; i++) {
      const ruleData = rules[i];
      if (ruleData.required) {
        isNoField = true;
        break;
      }
    }
    return isNoField;
  };
  const isEqual = (a, b) => {
    if (a === b) {
      return a !== 0 || 1 / a === 1 / b;
    }
    if (a == null || b == null) {
      return a === b;
    }
    var classNameA = toString.call(a), classNameB = toString.call(b);
    if (classNameA !== classNameB) {
      return false;
    }
    switch (classNameA) {
      case "[object RegExp]":
      case "[object String]":
        return "" + a === "" + b;
      case "[object Number]":
        if (+a !== +a) {
          return +b !== +b;
        }
        return +a === 0 ? 1 / +a === 1 / b : +a === +b;
      case "[object Date]":
      case "[object Boolean]":
        return +a === +b;
    }
    if (classNameA == "[object Object]") {
      var propsA = Object.getOwnPropertyNames(a), propsB = Object.getOwnPropertyNames(b);
      if (propsA.length != propsB.length) {
        return false;
      }
      for (var i = 0; i < propsA.length; i++) {
        var propName = propsA[i];
        if (a[propName] !== b[propName]) {
          return false;
        }
      }
      return true;
    }
    if (classNameA == "[object Array]") {
      if (a.toString() == b.toString()) {
        return true;
      }
      return false;
    }
  };
  const _sfc_main$2 = {
    name: "uniForms",
    emits: ["validate", "submit"],
    options: {
      virtualHost: true
    },
    props: {
      // 即将弃用
      value: {
        type: Object,
        default() {
          return null;
        }
      },
      // vue3 替换 value 属性
      modelValue: {
        type: Object,
        default() {
          return null;
        }
      },
      // 1.4.0 开始将不支持 v-model ，且废弃 value 和 modelValue
      model: {
        type: Object,
        default() {
          return null;
        }
      },
      // 表单校验规则
      rules: {
        type: Object,
        default() {
          return {};
        }
      },
      //校验错误信息提示方式 默认 undertext 取值 [undertext|toast|modal]
      errShowType: {
        type: String,
        default: "undertext"
      },
      // 校验触发器方式 默认 bind 取值 [bind|submit]
      validateTrigger: {
        type: String,
        default: "submit"
      },
      // label 位置，默认 left 取值  top/left
      labelPosition: {
        type: String,
        default: "left"
      },
      // label 宽度
      labelWidth: {
        type: [String, Number],
        default: ""
      },
      // label 居中方式，默认 left 取值 left/center/right
      labelAlign: {
        type: String,
        default: "left"
      },
      border: {
        type: Boolean,
        default: false
      }
    },
    provide() {
      return {
        uniForm: this
      };
    },
    data() {
      return {
        // 表单本地值的记录，不应该与传如的值进行关联
        formData: {},
        formRules: {}
      };
    },
    computed: {
      // 计算数据源变化的
      localData() {
        const localVal = this.model || this.modelValue || this.value;
        if (localVal) {
          return deepCopy(localVal);
        }
        return {};
      }
    },
    watch: {
      // 监听数据变化 ,暂时不使用，需要单独赋值
      // localData: {},
      // 监听规则变化
      rules: {
        handler: function(val, oldVal) {
          this.setRules(val);
        },
        deep: true,
        immediate: true
      }
    },
    created() {
      let getbinddata = getApp().$vm.$.appContext.config.globalProperties.binddata;
      if (!getbinddata) {
        getApp().$vm.$.appContext.config.globalProperties.binddata = function(name, value, formName) {
          if (formName) {
            this.$refs[formName].setValue(name, value);
          } else {
            let formVm;
            for (let i in this.$refs) {
              const vm = this.$refs[i];
              if (vm && vm.$options && vm.$options.name === "uniForms") {
                formVm = vm;
                break;
              }
            }
            if (!formVm)
              return formatAppLog("error", "at uni_modules/uni-forms/components/uni-forms/uni-forms.vue:182", "当前 uni-froms 组件缺少 ref 属性");
            formVm.setValue(name, value);
          }
        };
      }
      this.childrens = [];
      this.inputChildrens = [];
      this.setRules(this.rules);
    },
    methods: {
      /**
       * 外部调用方法
       * 设置规则 ，主要用于小程序自定义检验规则
       * @param {Array} rules 规则源数据
       */
      setRules(rules) {
        this.formRules = Object.assign({}, this.formRules, rules);
        this.validator = new SchemaValidator(rules);
      },
      /**
       * 外部调用方法
       * 设置数据，用于设置表单数据，公开给用户使用 ， 不支持在动态表单中使用
       * @param {Object} key
       * @param {Object} value
       */
      setValue(key, value) {
        let example = this.childrens.find((child) => child.name === key);
        if (!example)
          return null;
        this.formData[key] = getValue(key, value, this.formRules[key] && this.formRules[key].rules || []);
        return example.onFieldChange(this.formData[key]);
      },
      /**
       * 外部调用方法
       * 手动提交校验表单
       * 对整个表单进行校验的方法，参数为一个回调函数。
       * @param {Array} keepitem 保留不参与校验的字段
       * @param {type} callback 方法回调
       */
      validate(keepitem, callback) {
        return this.checkAll(this.formData, keepitem, callback);
      },
      /**
       * 外部调用方法
       * 部分表单校验
       * @param {Array|String} props 需要校验的字段
       * @param {Function} 回调函数
       */
      validateField(props = [], callback) {
        props = [].concat(props);
        let invalidFields = {};
        this.childrens.forEach((item) => {
          const name = realName(item.name);
          if (props.indexOf(name) !== -1) {
            invalidFields = Object.assign({}, invalidFields, {
              [name]: this.formData[name]
            });
          }
        });
        return this.checkAll(invalidFields, [], callback);
      },
      /**
       * 外部调用方法
       * 移除表单项的校验结果。传入待移除的表单项的 prop 属性或者 prop 组成的数组，如不传则移除整个表单的校验结果
       * @param {Array|String} props 需要移除校验的字段 ，不填为所有
       */
      clearValidate(props = []) {
        props = [].concat(props);
        this.childrens.forEach((item) => {
          if (props.length === 0) {
            item.errMsg = "";
          } else {
            const name = realName(item.name);
            if (props.indexOf(name) !== -1) {
              item.errMsg = "";
            }
          }
        });
      },
      /**
       * 外部调用方法 ，即将废弃
       * 手动提交校验表单
       * 对整个表单进行校验的方法，参数为一个回调函数。
       * @param {Array} keepitem 保留不参与校验的字段
       * @param {type} callback 方法回调
       */
      submit(keepitem, callback, type) {
        for (let i in this.dataValue) {
          const itemData = this.childrens.find((v) => v.name === i);
          if (itemData) {
            if (this.formData[i] === void 0) {
              this.formData[i] = this._getValue(i, this.dataValue[i]);
            }
          }
        }
        if (!type) {
          formatAppLog("warn", "at uni_modules/uni-forms/components/uni-forms/uni-forms.vue:289", "submit 方法即将废弃，请使用validate方法代替！");
        }
        return this.checkAll(this.formData, keepitem, callback, "submit");
      },
      // 校验所有
      async checkAll(invalidFields, keepitem, callback, type) {
        if (!this.validator)
          return;
        let childrens = [];
        for (let i in invalidFields) {
          const item = this.childrens.find((v) => realName(v.name) === i);
          if (item) {
            childrens.push(item);
          }
        }
        if (!callback && typeof keepitem === "function") {
          callback = keepitem;
        }
        let promise;
        if (!callback && typeof callback !== "function" && Promise) {
          promise = new Promise((resolve, reject) => {
            callback = function(valid, invalidFields2) {
              !valid ? resolve(invalidFields2) : reject(valid);
            };
          });
        }
        let results = [];
        let tempFormData = JSON.parse(JSON.stringify(invalidFields));
        for (let i in childrens) {
          const child = childrens[i];
          let name = realName(child.name);
          const result = await child.onFieldChange(tempFormData[name]);
          if (result) {
            results.push(result);
            if (this.errShowType === "toast" || this.errShowType === "modal")
              break;
          }
        }
        if (Array.isArray(results)) {
          if (results.length === 0)
            results = null;
        }
        if (Array.isArray(keepitem)) {
          keepitem.forEach((v) => {
            let vName = realName(v);
            let value = getDataValue(v, this.localData);
            if (value !== void 0) {
              tempFormData[vName] = value;
            }
          });
        }
        if (type === "submit") {
          this.$emit("submit", {
            detail: {
              value: tempFormData,
              errors: results
            }
          });
        } else {
          this.$emit("validate", results);
        }
        let resetFormData = {};
        resetFormData = rawData(tempFormData, this.name);
        callback && typeof callback === "function" && callback(results, resetFormData);
        if (promise && callback) {
          return promise;
        } else {
          return null;
        }
      },
      /**
       * 返回validate事件
       * @param {Object} result
       */
      validateCheck(result) {
        this.$emit("validate", result);
      },
      _getValue: getValue,
      _isRequiredField: isRequiredField,
      _setDataValue: setDataValue,
      _getDataValue: getDataValue,
      _realName: realName,
      _isRealName: isRealName,
      _isEqual: isEqual
    }
  };
  function _sfc_render$1(_ctx, _cache, $props, $setup, $data, $options) {
    return vue.openBlock(), vue.createElementBlock("view", { class: "uni-forms" }, [
      vue.createElementVNode("form", null, [
        vue.renderSlot(_ctx.$slots, "default", {}, void 0, true)
      ])
    ]);
  }
  const __easycom_2 = /* @__PURE__ */ _export_sfc(_sfc_main$2, [["render", _sfc_render$1], ["__scopeId", "data-v-9a1e3c32"], ["__file", "E:/NiuNiu/code/YZV25_PDA/uni_modules/uni-forms/components/uni-forms/uni-forms.vue"]]);
  const _sfc_main$1 = {
    data() {
      return {
        // 基础表单数据
        valiFormData: {
          gx: "1",
          jt: "",
          sb: "PDA",
          isOpen: 0,
          ip: "",
          deviceType: "",
          port: "6060",
          apiPath: "api/pda/scan"
        },
        // 校验规则
        rules: {
          gx: {
            rules: [{
              required: true,
              errorMessage: "工序不能为空"
            }]
          },
          jt: {
            rules: [{
              required: true,
              errorMessage: "机台不能为空"
            }]
          },
          sb: {
            rules: [{
              required: true,
              errorMessage: "设备不能为空"
            }]
          },
          ip: {
            rules: [{
              required: true,
              errorMessage: "本地服务器IP不能为空"
            }]
          },
          port: {
            rules: [{
              required: true,
              errorMessage: "端口不能为空"
            }]
          },
          deviceType: {
            rules: [{
              required: true,
              errorMessage: "设备类型不能为空"
            }]
          },
          apiPath: {
            rules: [{
              required: true,
              errorMessage: "接口名称不能为空"
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
        options: ["CNC", "CMT", "FSW"],
        currentTheme: "light"
      };
    },
    onLoad() {
      this.$dbUtils.selectDataList("dataDB", "mes", {
        id: 1
      }, "id", "desc").then((res) => {
        if (res.length == 0) {
          this.$dbUtils.addTabItem("dataDB", "mes", {
            gx: "测试",
            jt: "测试",
            sb: "测试",
            isOpen: 0,
            deviceType: "",
            ip: "192.168.60.36",
            port: "6060",
            apiPath: "api/pda/scan"
          }).then(
            (res3) => {
              this.valiFormData.gx = "1";
              this.valiFormData.jt = "测试";
              this.valiFormData.sb = "测试";
              this.valiFormData.isOpen = 0;
              this.valiFormData.deviceType = "";
              this.valiFormData.ip = "192.168.60.36";
              this.valiFormData.port = "6060";
              this.valiFormData.apiPath = "api/pda/scan";
            }
          );
        } else {
          formatAppLog("log", "at pages/index/index3.vue:181", res[0]);
          this.valiFormData.gx = res[0].gx;
          this.valiFormData.jt = res[0].jt;
          this.valiFormData.deviceType = res[0].deviceType;
          this.valiFormData.sb = res[0].sb;
          this.valiFormData.isOpen = res[0].isOpen;
          this.valiFormData.ip = res[0].ip;
          this.valiFormData.port = res[0].port || "6060";
          this.valiFormData.apiPath = res[0].apiPath || "api/pda/scan";
        }
      });
      const savedTheme = uni.getStorageSync("appTheme");
      if (savedTheme) {
        this.currentTheme = savedTheme;
      }
    },
    methods: {
      onPickerChange(e) {
        const index = e.detail.value;
        this.valiFormData.deviceType = this.options[index];
      },
      // 返回上一页
      goBack() {
        uni.navigateBack({
          delta: 1,
          fail: () => {
            uni.reLaunch({
              url: "/pages/index/index"
            });
          }
        });
      },
      // 切换主题
      switchTheme(theme) {
        this.currentTheme = theme;
        uni.setStorageSync("appTheme", theme);
        uni.showToast({
          title: "主题已切换",
          icon: "success"
        });
      },
      submit(ref) {
        this.$refs[ref].validate().then((res) => {
          formatAppLog("log", "at pages/index/index3.vue:229", "表单校验通过：" + JSON.stringify(res));
          const updateData = {
            ...res,
            isOpen: this.valiFormData.isOpen,
            sb: this.valiFormData.sb
          };
          formatAppLog("log", "at pages/index/index3.vue:236", "更新数据：" + JSON.stringify(updateData));
          this.$dbUtils.selectDataList("dataDB", "mes", { id: 1 }, "id", "desc").then((existRes) => {
            if (existRes.length > 0) {
              this.$dbUtils.updateSQL("dataDB", "mes", updateData, "id", 1).then(
                (res3) => {
                  uni.showToast({
                    title: "保存成功"
                  });
                  setTimeout(() => {
                    uni.navigateBack();
                  }, 500);
                }
              ).catch((err) => {
                formatAppLog("log", "at pages/index/index3.vue:252", "更新失败：", err);
                uni.showToast({
                  title: "保存失败：" + (err.message || err),
                  icon: "none"
                });
              });
            } else {
              this.$dbUtils.addTabItem("dataDB", "mes", { ...updateData, id: 1 }).then(
                (res3) => {
                  uni.showToast({
                    title: "保存成功"
                  });
                  setTimeout(() => {
                    uni.navigateBack();
                  }, 500);
                }
              ).catch((err) => {
                formatAppLog("log", "at pages/index/index3.vue:270", "插入失败：", err);
                uni.showToast({
                  title: "保存失败：" + (err.message || err),
                  icon: "none"
                });
              });
            }
          }).catch((err) => {
            formatAppLog("log", "at pages/index/index3.vue:278", "查询失败：", err);
            uni.showToast({
              title: "查询失败",
              icon: "none"
            });
          });
        }).catch((err) => {
          formatAppLog("log", "at pages/index/index3.vue:285", "表单校验失败：", err);
          uni.showToast({
            title: "请检查输入内容",
            icon: "none"
          });
        });
      }
    }
  };
  function _sfc_render(_ctx, _cache, $props, $setup, $data, $options) {
    const _component_uni_easyinput = resolveEasycom(vue.resolveDynamicComponent("uni-easyinput"), __easycom_0);
    const _component_uni_forms_item = resolveEasycom(vue.resolveDynamicComponent("uni-forms-item"), __easycom_1);
    const _component_uni_forms = resolveEasycom(vue.resolveDynamicComponent("uni-forms"), __easycom_2);
    return vue.openBlock(), vue.createElementBlock(
      "view",
      {
        class: vue.normalizeClass(["container", ["theme-" + $data.currentTheme]])
      },
      [
        vue.createCommentVNode(" 自定义导航栏 "),
        vue.createElementVNode("view", { class: "custom-nav" }, [
          vue.createElementVNode("view", {
            class: "nav-left",
            onClick: _cache[0] || (_cache[0] = (...args) => $options.goBack && $options.goBack(...args))
          }, [
            vue.createElementVNode("view", { class: "nav-icon back-icon" }),
            vue.createElementVNode("text", { class: "nav-text" }, "返回")
          ]),
          vue.createElementVNode("text", { class: "nav-title" }, "设置"),
          vue.createElementVNode("view", { class: "nav-right" })
        ]),
        vue.createElementVNode("view", { class: "content-wrapper" }, [
          vue.createElementVNode("view", { class: "example" }, [
            vue.createCommentVNode(" 基础用法，不包含校验规则 "),
            vue.createVNode(_component_uni_forms, {
              ref: "valiForm",
              rules: $data.rules,
              modelValue: $data.valiFormData
            }, {
              default: vue.withCtx(() => [
                vue.createVNode(_component_uni_forms_item, {
                  label: "机台号",
                  required: "",
                  name: "gx"
                }, {
                  default: vue.withCtx(() => [
                    vue.createVNode(_component_uni_easyinput, {
                      modelValue: $data.valiFormData.gx,
                      "onUpdate:modelValue": _cache[1] || (_cache[1] = ($event) => $data.valiFormData.gx = $event),
                      placeholder: "机台号"
                    }, null, 8, ["modelValue"])
                  ]),
                  _: 1
                  /* STABLE */
                }),
                vue.createVNode(_component_uni_forms_item, {
                  label: "机台名称",
                  required: "",
                  name: "jt"
                }, {
                  default: vue.withCtx(() => [
                    vue.createVNode(_component_uni_easyinput, {
                      modelValue: $data.valiFormData.jt,
                      "onUpdate:modelValue": _cache[2] || (_cache[2] = ($event) => $data.valiFormData.jt = $event),
                      placeholder: "机台名称"
                    }, null, 8, ["modelValue"])
                  ]),
                  _: 1
                  /* STABLE */
                }),
                vue.createVNode(_component_uni_forms_item, {
                  label: "设备类型",
                  required: "",
                  name: "deviceType"
                }, {
                  default: vue.withCtx(() => [
                    vue.createElementVNode("picker", {
                      range: $data.options,
                      onChange: _cache[3] || (_cache[3] = (...args) => $options.onPickerChange && $options.onPickerChange(...args)),
                      class: "picker"
                    }, [
                      vue.createElementVNode(
                        "view",
                        { class: "picker-text" },
                        vue.toDisplayString($data.valiFormData.deviceType || "请选择"),
                        1
                        /* TEXT */
                      )
                    ], 40, ["range"])
                  ]),
                  _: 1
                  /* STABLE */
                }),
                vue.createVNode(_component_uni_forms_item, {
                  label: "本地服务器IP",
                  required: "",
                  name: "ip"
                }, {
                  default: vue.withCtx(() => [
                    vue.createVNode(_component_uni_easyinput, {
                      modelValue: $data.valiFormData.ip,
                      "onUpdate:modelValue": _cache[4] || (_cache[4] = ($event) => $data.valiFormData.ip = $event),
                      placeholder: "请输入本地服务器IP"
                    }, null, 8, ["modelValue"])
                  ]),
                  _: 1
                  /* STABLE */
                }),
                vue.createVNode(_component_uni_forms_item, {
                  label: "端口",
                  required: "",
                  name: "port"
                }, {
                  default: vue.withCtx(() => [
                    vue.createVNode(_component_uni_easyinput, {
                      modelValue: $data.valiFormData.port,
                      "onUpdate:modelValue": _cache[5] || (_cache[5] = ($event) => $data.valiFormData.port = $event),
                      placeholder: "请输入端口，如：6060"
                    }, null, 8, ["modelValue"])
                  ]),
                  _: 1
                  /* STABLE */
                }),
                vue.createVNode(_component_uni_forms_item, {
                  label: "接口名称",
                  required: "",
                  name: "apiPath"
                }, {
                  default: vue.withCtx(() => [
                    vue.createVNode(_component_uni_easyinput, {
                      modelValue: $data.valiFormData.apiPath,
                      "onUpdate:modelValue": _cache[6] || (_cache[6] = ($event) => $data.valiFormData.apiPath = $event),
                      placeholder: "请输入接口名称，如：api/pda/scan"
                    }, null, 8, ["modelValue"])
                  ]),
                  _: 1
                  /* STABLE */
                }),
                vue.createCommentVNode(" 主题选择 "),
                vue.createElementVNode("view", { class: "theme-section" }, [
                  vue.createElementVNode("text", { class: "theme-label" }, "主题"),
                  vue.createElementVNode("view", { class: "theme-selector" }, [
                    vue.createElementVNode(
                      "view",
                      {
                        class: vue.normalizeClass(["theme-option", { "active": $data.currentTheme === "light" }]),
                        onClick: _cache[7] || (_cache[7] = vue.withModifiers(($event) => $options.switchTheme("light"), ["stop"]))
                      },
                      [
                        vue.createElementVNode("view", { class: "theme-preview theme-light-preview" }),
                        vue.createElementVNode("text", { class: "theme-name" }, "浅色")
                      ],
                      2
                      /* CLASS */
                    ),
                    vue.createElementVNode(
                      "view",
                      {
                        class: vue.normalizeClass(["theme-option", { "active": $data.currentTheme === "dark" }]),
                        onClick: _cache[8] || (_cache[8] = vue.withModifiers(($event) => $options.switchTheme("dark"), ["stop"]))
                      },
                      [
                        vue.createElementVNode("view", { class: "theme-preview theme-dark-preview" }),
                        vue.createElementVNode("text", { class: "theme-name" }, "深色")
                      ],
                      2
                      /* CLASS */
                    ),
                    vue.createElementVNode(
                      "view",
                      {
                        class: vue.normalizeClass(["theme-option", { "active": $data.currentTheme === "orange" }]),
                        onClick: _cache[9] || (_cache[9] = vue.withModifiers(($event) => $options.switchTheme("orange"), ["stop"]))
                      },
                      [
                        vue.createElementVNode("view", { class: "theme-preview theme-orange-preview" }),
                        vue.createElementVNode("text", { class: "theme-name" }, "工业橙")
                      ],
                      2
                      /* CLASS */
                    )
                  ])
                ])
              ]),
              _: 1
              /* STABLE */
            }, 8, ["rules", "modelValue"]),
            vue.createElementVNode("button", {
              type: "primary",
              onClick: _cache[10] || (_cache[10] = ($event) => $options.submit("valiForm"))
            }, "提交")
          ])
        ])
      ],
      2
      /* CLASS */
    );
  }
  const PagesIndexIndex3 = /* @__PURE__ */ _export_sfc(_sfc_main$1, [["render", _sfc_render], ["__file", "E:/NiuNiu/code/YZV25_PDA/pages/index/index3.vue"]]);
  __definePage("pages/index/index", PagesIndexIndex);
  __definePage("pages/index/index2", PagesIndexIndex2);
  __definePage("pages/index/index3", PagesIndexIndex3);
  const _sfc_main = {
    onLaunch: function() {
      formatAppLog("log", "at App.vue:4", "App Launch");
      this.$dbUtils.openDb("dataDB");
      this.$dbUtils.init("dataDB", [
        {
          tableName: "data",
          sql: `CREATE TABLE "data" (
								"id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
								  "scancode" TEXT,
								  "ctime" timestamp  DEFAULT (datetime(CURRENT_TIMESTAMP,'localtime')),
								  "isUpdate" INTEGER DEFAULT 0
								  );`
        },
        {
          tableName: "mes",
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
      this.checkAndAddColumns();
    },
    methods: {
      // 检查并添加缺失的字段
      checkAndAddColumns() {
        this.$dbUtils.selectDataList("dataDB", "mes", { id: 1 }, "id", "desc").then((res) => {
          formatAppLog("log", "at App.vue:39", "检查数据库字段...");
        }).catch((err) => {
          formatAppLog("log", "at App.vue:41", "数据库查询错误，可能需要添加新字段");
        });
        plus.sqlite.executeSql({
          name: "dataDB",
          sql: `ALTER TABLE mes ADD COLUMN deviceType TEXT DEFAULT ''`,
          success: (e) => {
            formatAppLog("log", "at App.vue:49", "成功添加 deviceType 字段");
          },
          fail: (e) => {
            formatAppLog("log", "at App.vue:53", "deviceType 字段已存在或添加失败:", e.message);
          }
        });
        plus.sqlite.executeSql({
          name: "dataDB",
          sql: `ALTER TABLE mes ADD COLUMN port TEXT DEFAULT '5193'`,
          success: (e) => {
            formatAppLog("log", "at App.vue:64", "成功添加 port 字段");
          },
          fail: (e) => {
            formatAppLog("log", "at App.vue:68", "port 字段已存在或添加失败:", e.message);
          }
        });
        plus.sqlite.executeSql({
          name: "dataDB",
          sql: `ALTER TABLE mes ADD COLUMN apiPath TEXT DEFAULT 'api/pda/scan'`,
          success: (e) => {
            formatAppLog("log", "at App.vue:77", "成功添加 apiPath 字段");
          },
          fail: (e) => {
            formatAppLog("log", "at App.vue:81", "apiPath 字段已存在或添加失败:", e.message);
          }
        });
      }
    },
    onShow: function() {
      formatAppLog("log", "at App.vue:87", "App Show");
    },
    onHide: function() {
      formatAppLog("log", "at App.vue:90", "App Hide");
    }
  };
  const App = /* @__PURE__ */ _export_sfc(_sfc_main, [["__file", "E:/NiuNiu/code/YZV25_PDA/App.vue"]]);
  const openDb = (name) => {
    return new Promise((resolve, reject) => {
      plus.sqlite.openDatabase({
        name,
        //数据库名称
        path: `_doc/${name}.db`,
        //数据库地址
        success(e) {
          formatAppLog("info", "at uni_modules/zjy-sqlite-manage/components/zjy-sqlite-manage/dbUtils.js:11", "11", e);
          resolve(e);
        },
        fail(e) {
          formatAppLog("info", "at uni_modules/zjy-sqlite-manage/components/zjy-sqlite-manage/dbUtils.js:15", "22", e);
          reject(e);
        }
      });
    });
  };
  const init = (name, tableSqls) => {
    formatAppLog("info", "at uni_modules/zjy-sqlite-manage/components/zjy-sqlite-manage/dbUtils.js:23", "数据库：" + name);
    for (let i = 0; i < tableSqls.length; i++) {
      let data = tableSqls[i];
      isTable(name, data.tableName).then((res) => {
        formatAppLog("info", "at uni_modules/zjy-sqlite-manage/components/zjy-sqlite-manage/dbUtils.js:28", "表已存在res：" + res);
        formatAppLog("info", "at uni_modules/zjy-sqlite-manage/components/zjy-sqlite-manage/dbUtils.js:29", "表已存在：" + data.tableName);
        if (!res) {
          formatAppLog("info", "at uni_modules/zjy-sqlite-manage/components/zjy-sqlite-manage/dbUtils.js:31", "初始化表：", data.tableName);
          addTab(name, data.sql);
        }
      }).catch((e) => {
        formatAppLog("info", "at uni_modules/zjy-sqlite-manage/components/zjy-sqlite-manage/dbUtils.js:35", "初始化表：", data.tableName);
        addTab(name, data.sql);
      });
    }
  };
  const getTable = (name) => {
    return new Promise((resolve, reject) => {
      plus.sqlite.selectSql({
        name,
        sql: "select * FROM sqlite_master where type='table'",
        success(e) {
          formatAppLog("log", "at uni_modules/zjy-sqlite-manage/components/zjy-sqlite-manage/dbUtils.js:52", "getTable", e);
          resolve(e);
        },
        fail(e) {
          formatAppLog("log", "at uni_modules/zjy-sqlite-manage/components/zjy-sqlite-manage/dbUtils.js:56", e);
          reject(e);
        }
      });
    });
  };
  const getCount = (name, tabName) => {
    return new Promise((resolve, reject) => {
      plus.sqlite.selectSql({
        name,
        sql: "select count(*) as num from " + tabName,
        success(e) {
          resolve(e);
        },
        fail(e) {
          reject(e);
        }
      });
    });
  };
  const isTable = (name, tabName) => {
    formatAppLog("info", "at uni_modules/zjy-sqlite-manage/components/zjy-sqlite-manage/dbUtils.js:80", "name  tabName  ", name, tabName);
    return new Promise((resolve, reject) => {
      plus.sqlite.selectSql({
        name,
        sql: `select count(*) as isTable FROM sqlite_master where type='table' and name='${tabName}'`,
        success(e) {
          resolve(e[0].isTable ? true : false);
        },
        fail(e) {
          formatAppLog("log", "at uni_modules/zjy-sqlite-manage/components/zjy-sqlite-manage/dbUtils.js:89", e);
          reject(e);
        }
      });
    });
  };
  const updateSQL = (name, tabName, setData, setName, setVal) => {
    if (JSON.stringify(setData) !== "{}") {
      let dataKeys = Object.keys(setData);
      let setStr = "";
      dataKeys.forEach((item, index) => {
        formatAppLog("log", "at uni_modules/zjy-sqlite-manage/components/zjy-sqlite-manage/dbUtils.js:101", setData[item]);
        setStr += `${item} = ${JSON.stringify(setData[item])}${dataKeys.length - 1 !== index ? "," : ""}`;
      });
      return new Promise((resolve, reject) => {
        plus.sqlite.executeSql({
          name,
          sql: `update ${tabName} set ${setStr} where ${setName} = "${setVal}"`,
          success(e) {
            resolve(e);
          },
          fail(e) {
            formatAppLog("log", "at uni_modules/zjy-sqlite-manage/components/zjy-sqlite-manage/dbUtils.js:113", e);
            reject(e);
          }
        });
      });
    } else {
      return new Promise((resolve, reject) => {
        reject("错误");
      });
    }
  };
  const delData = (name, tabName, setData) => {
    if (JSON.stringify(setData) !== "{}") {
      let dataKeys = Object.keys(setData);
      let setStr = "";
      dataKeys.forEach((item, index) => {
        formatAppLog("log", "at uni_modules/zjy-sqlite-manage/components/zjy-sqlite-manage/dbUtils.js:131", setData[item]);
        setStr += `${item}=${JSON.stringify(setData[item])}${dataKeys.length - 1 !== index ? " and " : ""}`;
      });
      return new Promise((resolve, reject) => {
        formatAppLog("info", "at uni_modules/zjy-sqlite-manage/components/zjy-sqlite-manage/dbUtils.js:137", `delete from ${tabName} where ${setStr}`);
        plus.sqlite.executeSql({
          name,
          sql: `delete from ${tabName} where ${setStr}`,
          success(e) {
            resolve(e);
          },
          fail(e) {
            reject(e);
          }
        });
      });
    } else {
      return new Promise((resolve, reject) => {
        reject("错误");
      });
    }
  };
  const closeSQL = (name) => {
    return new Promise((resolve, reject) => {
      plus.sqlite.closeDatabase({
        name: "pop",
        success(e) {
          resolve(e);
        },
        fail(e) {
          reject(e);
        }
      });
    });
  };
  const isOpen = (name) => {
    let open = plus.sqlite.isOpenDatabase({
      name,
      path: `_doc/${name}.db`
    });
    return open;
  };
  const delTable = (name, tabName) => {
    return new Promise((resolve, reject) => {
      plus.sqlite.executeSql({
        name,
        sql: `DROP TABLE ${tabName}`,
        success(e) {
          resolve(e);
        },
        fail(e) {
          formatAppLog("log", "at uni_modules/zjy-sqlite-manage/components/zjy-sqlite-manage/dbUtils.js:191", e);
          reject(e);
        }
      });
    });
  };
  const addTab = (name, sql) => {
    return new Promise((resolve, reject) => {
      plus.sqlite.executeSql({
        name,
        // sql: 'create table if not exists dataList("list" INTEGER PRIMARY KEY AUTOINCREMENT,"id" TEXT,"name" TEXT,"gender" TEXT,"avatar" TEXT)',
        // sql: `create table if not exists ${tabName}("chat_i" INTEGER PRIMARY KEY AUTOINCREMENT,"local_id" TEXT NOT NULL UNIQUE,"id" TEXT,"chat_friend_id" TEXT,"content" INTEGER)`,
        sql,
        success(e) {
          resolve(e);
        },
        fail(e) {
          formatAppLog("log", "at uni_modules/zjy-sqlite-manage/components/zjy-sqlite-manage/dbUtils.js:210", e);
          reject(e);
        }
      });
    });
  };
  const addTabItem = (name, tabName, obj) => {
    if (obj) {
      let keys = Object.keys(obj);
      let keyStr = keys.toString();
      let valStr = "";
      keys.forEach((item, index) => {
        if (keys.length - 1 == index) {
          valStr += '"' + obj[item] + '"';
        } else {
          valStr += '"' + obj[item] + '",';
        }
      });
      formatAppLog("log", "at uni_modules/zjy-sqlite-manage/components/zjy-sqlite-manage/dbUtils.js:234", valStr);
      let sqlStr = `insert into ${tabName}(${keyStr}) values(${valStr})`;
      formatAppLog("log", "at uni_modules/zjy-sqlite-manage/components/zjy-sqlite-manage/dbUtils.js:236", sqlStr);
      return new Promise((resolve, reject) => {
        plus.sqlite.executeSql({
          name,
          sql: sqlStr,
          success(e) {
            resolve(e);
          },
          fail(e) {
            formatAppLog("log", "at uni_modules/zjy-sqlite-manage/components/zjy-sqlite-manage/dbUtils.js:245", e);
            reject(e);
          }
        });
      });
    } else {
      return new Promise((resolve, reject) => {
        reject("错误");
      });
    }
  };
  const mergeSql = (name, tabName, tabs) => {
    if (!tabs || tabs.length == 0) {
      return new Promise((resolve, reject) => {
        reject("错误");
      });
    }
    let itemValStr = "";
    tabs.forEach((item, index) => {
      let itemKey = Object.keys(item);
      let itemVal = "";
      itemKey.forEach((key, i) => {
        if (itemKey.length - 1 == i) {
          if (typeof item[key] == "object") {
            itemVal += `'${JSON.stringify(item[key])}'`;
          } else {
            itemVal += `'${item[key]}'`;
          }
        } else {
          if (typeof item[key] == "object") {
            itemVal += `'${JSON.stringify(item[key])}',`;
          } else {
            itemVal += `'${item[key]}',`;
          }
        }
      });
      if (tabs.length - 1 == index) {
        itemValStr += "(" + itemVal + ")";
      } else {
        itemValStr += "(" + itemVal + "),";
      }
    });
    let keys = Object.keys(tabs[0]);
    let keyStr = keys.toString();
    return new Promise((resolve, reject) => {
      plus.sqlite.executeSql({
        name,
        sql: `insert or ignore into ${tabName} (${keyStr}) values ${itemValStr}`,
        success(e) {
          resolve(e);
        },
        fail(e) {
          formatAppLog("log", "at uni_modules/zjy-sqlite-manage/components/zjy-sqlite-manage/dbUtils.js:298", e);
          reject(e);
        }
      });
    });
  };
  const getDataList = async (name, tabName, num, size, byName, byType) => {
    let count = 0;
    let sql = "";
    let numindex = 0;
    await getCount(name, tabName).then((resNum) => {
      count = Math.ceil(resNum[0].num / size);
    });
    if ((num - 1) * size == 0) {
      numindex = 0;
    } else {
      numindex = (num - 1) * size + 1;
    }
    sql = `select * from ${tabName}`;
    if (byName && byType) {
      sql += ` order by ${byName} ${byType}`;
    }
    sql += ` limit ${numindex},${size}`;
    if (count < num - 1) {
      return new Promise((resolve, reject) => {
        reject("无数据");
      });
    } else {
      return new Promise((resolve, reject) => {
        plus.sqlite.selectSql({
          name,
          // sql: "select * from userInfo limit 3 offset 3",
          sql,
          success(e) {
            resolve(e);
          },
          fail(e) {
            reject(e);
          }
        });
      });
    }
  };
  const selectDataList = (name, tabName, setData, byName, byType) => {
    let setStr = "";
    let sql = "";
    if (JSON.stringify(setData) !== "{}") {
      let dataKeys = Object.keys(setData);
      dataKeys.forEach((item, index) => {
        formatAppLog("log", "at uni_modules/zjy-sqlite-manage/components/zjy-sqlite-manage/dbUtils.js:354", setData[item]);
        setStr += `${item}=${JSON.stringify(setData[item])}${dataKeys.length - 1 !== index ? " and " : ""}`;
      });
      sql = `select * from ${tabName} where ${setStr}`;
    } else {
      sql = `select * from ${tabName}`;
    }
    if (byName && byType) {
      sql += ` order by ${byName} ${byType}`;
    }
    if (tabName !== void 0) {
      return new Promise((resolve, reject) => {
        plus.sqlite.selectSql({
          name,
          sql,
          success(e) {
            resolve(e);
          },
          fail(e) {
            formatAppLog("log", "at uni_modules/zjy-sqlite-manage/components/zjy-sqlite-manage/dbUtils.js:376", e);
            reject(e);
          }
        });
      });
    } else {
      return new Promise((resolve, reject) => {
        reject("错误");
      });
    }
  };
  const selectDataListByLike = (name, tabName, colum, value, byName, byType) => {
    let sql = `select * from ${tabName} where ${colum} like '%${value}%'`;
    if (byName && byType) {
      sql += ` order by ${byName} ${byType}`;
    }
    if (tabName !== void 0) {
      return new Promise((resolve, reject) => {
        plus.sqlite.selectSql({
          name,
          sql,
          success(e) {
            resolve(e);
          },
          fail(e) {
            formatAppLog("log", "at uni_modules/zjy-sqlite-manage/components/zjy-sqlite-manage/dbUtils.js:404", e);
            reject(e);
          }
        });
      });
    } else {
      return new Promise((resolve, reject) => {
        reject("错误");
      });
    }
  };
  const selectCount = (name, tabName, setData) => {
    let setStr = "";
    let sql = "";
    if (JSON.stringify(setData) !== "{}") {
      let dataKeys = Object.keys(setData);
      dataKeys.forEach((item, index) => {
        formatAppLog("log", "at uni_modules/zjy-sqlite-manage/components/zjy-sqlite-manage/dbUtils.js:422", setData[item]);
        setStr += `${item}=${JSON.stringify(setData[item])}${dataKeys.length - 1 !== index ? " and " : ""}`;
      });
      sql = `SELECT COUNT(*) AS count FROM ${tabName} where ${setStr}`;
    } else {
      sql = `SELECT COUNT(*) AS count FROM ${tabName}`;
    }
    if (tabName !== void 0) {
      return new Promise((resolve, reject) => {
        plus.sqlite.selectSql({
          name,
          sql,
          success(e) {
            resolve(e);
          },
          fail(e) {
            formatAppLog("log", "at uni_modules/zjy-sqlite-manage/components/zjy-sqlite-manage/dbUtils.js:441", e);
            reject(e);
          }
        });
      });
    } else {
      return new Promise((resolve, reject) => {
        reject("错误");
      });
    }
  };
  const dbUtils = {
    openDb,
    //打开数据库
    init,
    //初始化数据库
    getTable,
    //获取所有的表信息
    getCount,
    //查询表数据总条数
    isTable,
    //表是否存在
    updateSQL,
    //修改数据
    delData,
    //删除数据库数据
    closeSQL,
    //关闭数据库
    isOpen,
    //监听数据库是否开启
    delTable,
    //删除表
    addTab,
    //创建表
    addTabItem,
    //添加数据
    mergeSql,
    //合并数据
    getDataList,
    //获取分页数据库数据
    selectDataList,
    //查询数据库数据
    selectDataListByLike,
    //模糊查询数据库数据
    selectCount
    //查询数据条数
  };
  function createApp() {
    const app = vue.createVueApp(App);
    app.config.globalProperties.$dbUtils = dbUtils;
    return {
      app
    };
  }
  const { app: __app__, Vuex: __Vuex__, Pinia: __Pinia__ } = createApp();
  uni.Vuex = __Vuex__;
  uni.Pinia = __Pinia__;
  __app__.provide("__globalStyles", __uniConfig.styles);
  __app__._component.mpType = "app";
  __app__._component.render = () => {
  };
  __app__.mount("#app");
})(Vue);
