const { contextBridge, ipcRenderer } = require('electron')


contextBridge.exposeInMainWorld('electronApi', {
    node: () => process.versions.node,
    chrome: () => process.versions.chrome,
    electron: () => process.versions.electron,
    ping: () => ipcRenderer.invoke('ping'),
    openImageFile:()=>ipcRenderer.invoke('dialog:openImageFile'),
    saveProject:(content)=>ipcRenderer.invoke('saveProject',content),
    loadProject:()=>ipcRenderer.invoke('loadProject',),
    exportToJSON:(content)=>ipcRenderer.invoke('exportToJSON',content),
    // 除函数之外，我们也可以暴露变量
  })