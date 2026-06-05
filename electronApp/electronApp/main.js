const { app, BrowserWindow, ipcMain, dialog, Menu } = require("electron");
const path = require("node:path");
const imageToBase64 = require("image-to-base64");
const fs = require("fs").promises;
const createWindow = () => {
  const win = new BrowserWindow({
    width: 800,
    height: 600,
    webPreferences: {
      webSecurity: false,
      preload: path.join(__dirname, "preload.js"),
    },
  });
  win.maximize();
  win.loadFile("./public/index.html");
  // Menu.setApplicationMenu(null);
};

const handleOpenImageFile = async () => {
  const { canceled, filePaths } = await dialog.showOpenDialog({
    title: "选择背景图片",
    filters: [{ name: "Images", extensions: ["jpg", "png", "gif"] }],
  });
  if (!canceled) {
    const ext=path.extname(filePaths[0]).replace('.','');
    const base64 = await imageToBase64(filePaths[0]);
    console.log(base64);
    return `data:image/${ext};base64,`+base64
  }
};

const saveProject = async (content) => {
  // console.log(content)
  const { canceled, filePath } = await dialog.showSaveDialog({
    title: "保存工程",
    filters: [{ name: "工程文件", extensions: ["prj"] }],
  });
  // console.log(canceled,filePath ,content)
  if (!canceled) {
    // return filePaths[0]
    await fs.writeFile(filePath.toString(), content);
    // try {
    //   fs.writeFileSync(filePath.toString(), content);
    // } catch (error) {
    //   console.log(error);
    // }
  }
};

const handleLoadProject = async () => {
  const { canceled, filePaths } = await dialog.showOpenDialog({
    title: "打开工程文件",
    filters: [{ name: "工程文件", extensions: ["prj"] }],
  });
  if (!canceled) {
    const data = await fs.readFile(filePaths[0]);

    return JSON.parse(data);
  }
};

const handleExportToJSON = async (content) => {
  const { canceled, filePath } = await dialog.showSaveDialog({
    title: "导出json",
    filters: [{ name: "json文件", extensions: ["json"] }],
  });
  // console.log(canceled,filePath ,content)
  if (!canceled) {
    // return filePaths[0]
    await fs.writeFile(filePath.toString(), JSON.parse(content));
    // try {
    //   fs.writeFileSync(filePath.toString(), content);
    // } catch (error) {
    //   console.log(error);
    // }
  }
};

app.whenReady().then(() => {
  ipcMain.handle("dialog:openImageFile", handleOpenImageFile);
  ipcMain.handle("saveProject", (event, content) => saveProject(content));
  ipcMain.handle("loadProject", handleLoadProject);
  ipcMain.handle("exportToJSON", (event, content) =>
    handleExportToJSON(content)
  );
  createWindow();
});

app.on("window-all-closed", () => {
  if (process.platform !== "darwin") app.quit();
});
