<template>

  <el-container class="drawer-container">
    <el-header style="display: flex; align-items: center;" height="48px">

      <div style="flex-basis: 240px;">
        <el-button-group class="ml-4">
          <el-button type="primary" @click="onLoadProject">打开</el-button>
          <el-button type="primary" @click="onSaveProject">保存</el-button>
          <el-button type="primary" @click="onExportShapeToJSON">导出</el-button>
        </el-button-group>

      </div>
      <div style="flex-grow: 1; display: flex;justify-content: center;  ">
        <el-button type="primary" @click="onFileChange">选择背景图片</el-button>
      </div>
      <div style="flex-basis: 240px;flex-grow: 0;display: flex; flex-direction: row-reverse;">
        <div>{{ state.mouseX }},{{ state.mouseY }}</div>

      </div>
    </el-header>
    <el-container>
      <el-main style="display: flex;position: relative; padding-left: 0px;background-color: rgba(230, 227, 227, 0.8);">
        <div class="drawer-toolbar">
          <el-radio-group v-model="state.shapeType">
            <el-radio-button label="移动" :value="-2" />
            <el-radio-button label="选择" :value="-1" />
            <el-radio-button label="点位" :value="OPTION_POINT" />
            <el-radio-button label="矩形" :value="OPTION_RECT" />
            <el-radio-button label="多边形" :value="OPTION_POLYGON" />
            <el-radio-button label="路径" :value="OPTION_PATH" />
          </el-radio-group>
        </div>

        <div class="drawer-main" ref="drawerMain"
          style="box-shadow: rgba(60, 64, 67, 0.3) 0px 1px 2px 0px, rgba(60, 64, 67, 0.15) 0px 1px 3px 1px; ">
          <canvas id="drawer" ref="drawer" style="height: 100%; width: 100%;">


          </canvas>
        </div>

      </el-main>
      <el-aside width="420px" style="padding: 5px;">
        <div style="height: 32; display: flex;justify-content: center; align-items: center;">

          <el-form label-width="160px">
            <el-form-item label="选择图形">
              <el-select v-model="state.selectShapeId" clearable placeholder="Select" style="width: 240px"
                @change="onSelectShapeId">
                <el-option v-for="item in state.shapeIdList" :key="item" :label="item" :value="item" />
              </el-select>
            </el-form-item>
            <el-form-item label="编号">
              <el-input v-model="state.shapeData.no" />
            </el-form-item>
            <el-form-item label="名称">
              <el-input v-model="state.shapeData.name" />
            </el-form-item>
            <el-form-item label="区域" v-if="state.shapeData.shapeType !== OPTION_PATH">
              <el-input v-model="state.shapeData.area" />
            </el-form-item>


            <el-form-item label="权重" v-if="state.shapeData.shapeType === OPTION_PATH">
              <el-input-number v-model="state.shapeData.weight" :min="0" />
            </el-form-item>
            <el-form-item label="起始点" v-if="state.shapeData.shapeType === OPTION_PATH">
              <el-select v-model="state.shapeData.start" filterable allow-create default-first-option @change="() => {
                const item = state.pathPoints[state.shapeData.start]//.find(item => item.no === extraData.start);

                state.shapeData.startType = item?.type
              }">
                <el-option v-for="item in state.pathPoints" :key="item.no" :label="item.name" :value="item.no" />
              </el-select>


            </el-form-item>
            <el-form-item label="起始点类型" v-if="state.shapeData.shapeType === OPTION_PATH">
              <el-radio-group v-model="state.shapeData.startType">
                <el-radio-button label="店门口" :value="0" size="small" />
                <el-radio-button label="楼梯口" :value="1" size="small" />
                <el-radio-button label="出入口" :value="2" size="small" />
              </el-radio-group>



            </el-form-item>
            <el-form-item label="起始点区域" v-if="state.shapeData.shapeType === OPTION_PATH">
              <el-input v-model="state.shapeData.startArea" />
            </el-form-item>
            <el-form-item label="起始关联店点位"
              v-if="state.shapeData.shapeType === OPTION_PATH && state.shapeData.startType === 0">
              <el-select v-model="state.shapeData.startLink" filterable allow-create default-first-option>
                <el-option v-for=" item in getPointList()" :key="item.no" :label="item.name" :value="item.no" />
              </el-select>
            </el-form-item>
            <el-form-item label="结束点" v-if="state.shapeData.shapeType === OPTION_PATH">

              <el-select v-model="state.shapeData.end" filterable allow-create default-first-option @change="() => {
                const item = state.pathPoints[state.shapeData.end]//.find(item => item.no === extraData.end);

                state.shapeData.endType = item?.type
              }">
                <el-option v-for="item in state.pathPoints" :key="item.no" :label="item.name" :value="item.no" />
              </el-select>
            </el-form-item>

            <el-form-item label="结束点类型" v-if="state.shapeData.shapeType === OPTION_PATH">

              <el-radio-group v-model="state.shapeData.endType">
                <el-radio-button label="店门口" :value="0" size="small" />
                <el-radio-button label="楼梯口" :value="1" size="small" />
                <el-radio-button label="出入口" :value="2" size="small" />
              </el-radio-group>
            </el-form-item>
            <el-form-item label="结束关联店点位"
              v-if="state.shapeData.shapeType === OPTION_PATH && state.shapeData.endType === 0">
              <el-select v-model="state.shapeData.endLink" filterable allow-create default-first-option>
                <el-option v-for="item in getPointList()" :key="item.no" :label="item.name" :value="item.no" />
              </el-select>
            </el-form-item>
            <el-form-item>
              <div class="dialog-footer">
                <el-button type="danger">删除</el-button>
                <el-button type="primary" @click="save">
                  设置
                </el-button>
              </div>
            </el-form-item>

          </el-form>

        </div>
        <div>



        </div>

      </el-aside>
    </el-container>
  </el-container>
</template>

<script setup>
import { nanoid } from 'nanoid'
import * as fabric from 'fabric'
import { onMounted, reactive, ref, watch } from 'vue';
// import { ElMessage } from 'element-plus'

const OPTION_POINT = 0;
const OPTION_RECT = 1;
const OPTION_PATH = 2;
const OPTION_PATH_POINT = 3;
const OPTION_POLYGON = 4;

const drawerMain = ref(null)
const drawer = ref(null)



const state = reactive({
  shapJson: null,

  fabricCanvas: null,


  startPoint: null,

  mousebuttonDown: false,

  mouseX: null,
  mouseY: null,

  drawing: false,


  shapeType: -2, //0 点  1  矩形 2 路径

  pathDialogShow: false,
  shapeDialogShow: false,
  shapeData: {
    no: null,
    name: null
  },
  pathPoints: [],

  shapeIdList: [],
  selectShapeId: null

});

let curShape = null

let curDrawingShape = null;



const genID = (shapeType) => {

  let id = ''
  switch (shapeType) {

    case OPTION_POINT:
      id = 'POINT_' + nanoid(10);
      break;
    case OPTION_RECT:
      id = 'RECT_' + nanoid(10);
      break;
    case OPTION_POLYGON:
      id = 'POLYGON_' + nanoid(10);
      break;
    case OPTION_PATH:
      id = 'PATH_' + nanoid(10);
      break;
    case OPTION_PATH_POINT:
      id = 'PATH_POINT_' + nanoid(10);
      break;

  }

  state.shapeIdList.push(id);
  return id

}
const getObjectByID = (itemID) => {

  const shapes = state.fabricCanvas.getObjects();

  const shape = shapes.find(s => s.itemID === itemID);
  return shape;

}

const getPointList = () => {
  const shapes = state.fabricCanvas.getObjects();

  const points = shapes.filter(o => o.extraData.shapeType === OPTION_POINT)

  return points.map(p => { return { id: p.extraData.no, name: p.extraData.name } })

}


const init = async () => {




  const fabricCanvas = new fabric.Canvas(drawer.value, {
    backgroundColor: "#fff",
    width: drawerMain.value.clientWidth,
    height: drawerMain.value.clientHeight,
    renderOnAddRemove: true,
    altSelectionKey: 'altKey',

    selection: false,
    fireRightClick: true, // 启用右键，button的数字为3
    stopContextMenu: true, // 禁止默认右键菜单,
    selectionKey: 'ctrlKey',
    originX: 'left',
    originY: 'top'
  });

  fabricCanvas.on("mouse:dblclick", function (e) {

    if (state.shapeType !== -1 && state.shapeType !== -2) {
      return
    }
    if (!e.target) {
      return
    }


    const shape = e.target;
    // alert(shape.type)
    const extraData = shape.extraData;

    state.selectShapeId = shape.itemID
    state.shapeData = { ...extraData };
    curShape = shape;


  });


  fabricCanvas.on("mouse:wheel", function (e) {

    var zoom = (e.e.deltaY > 0 ? -0.1 : 0.1) + fabricCanvas.getZoom();
    zoom = Math.max(0.1, zoom); //最小为原来的1/10
    zoom = Math.min(3, zoom); //最大是原来的3倍
    var zoomPoint = new fabric.Point(e.e.pageX, e.e.pageY);
    fabricCanvas.zoomToPoint(zoomPoint, zoom);

  })

  fabricCanvas.on('mouse:down', function (e) {
    if (e.e.buttons === 2) {
      return
    }
    if (e.target) {
      return;
    }
    state.startPoint = e.scenePoint
    state.mousebuttonDown = true;
    switch (state.shapeType) {

      case OPTION_POINT: {
        if (e.target) {
          return;
        }
        state.drawing = true;
        const shape = new fabric.Circle({
          fill: 'red',
          radius: 6,
          left: state.startPoint.x,
          top: state.startPoint.y,
          originX: 'center', // x轴方向以中心点为原点
          originY: 'center', // y轴方向以中心点为原点
          lockMovementY: true,
          lockMovementX: true,
          hasControls: false, // 不显示控制器
          // hasBorders: false, // 不显示控制器的边
          // selectable: false,

        });
        shape.itemID = genID(state.shapeType);
        shape.extraData = { shapeType: state.shapeType };
        fabricCanvas.add(shape)
        curDrawingShape = shape;
      }
        break;
      case OPTION_RECT: {//
        state.drawing = true;
        const shape = new fabric.Rect({
          fill: null,
          left: state.startPoint.x,
          top: state.startPoint.y,
          width: 0,
          height: 0,
          objectCaching: false,

          strokeWidth: 2, // 边框大小
          stroke: 'red', // 边框颜色

        });
        shape.itemID = genID(state.shapeType);
        shape.extraData = { shapeType: state.shapeType };
        fabricCanvas.add(shape)
        curDrawingShape = shape;
      }
        break;
      case OPTION_PATH: { //折线

        state.drawing = true;
        if (!curDrawingShape) {
          const shape = new fabric.Polyline([{ x: state.startPoint.x, y: state.startPoint.y }], {
            strokeWidth: 2, // 边框大小
            stroke: 'red', // 边框颜色
            fill: null,
            hasControls: false, // 不显示控制器
            objectCaching: false,
            lockMovementY: true,
            lockMovementX: true,
          });
          shape.itemID = genID(state.shapeType);
          shape.extraData = { shapeType: state.shapeType };
          fabricCanvas.add(shape);
          curDrawingShape = shape;
        }

        curDrawingShape.points.push({ x: state.startPoint.x, y: state.startPoint.y })
        // curShape.setDimensions();
      }
        break;
      case OPTION_POLYGON: {


        state.drawing = true;
        if (!curDrawingShape) {
          const shape = new fabric.Polygon([{ x: state.startPoint.x, y: state.startPoint.y }], {
            strokeWidth: 2, // 边框大小
            stroke: 'red', // 边框颜色
            fill: null,
            hasControls: false, // 不显示控制器
            objectCaching: false,
            // lockMovementY: true,
            // lockMovementX: true,
            selectable: true,

          });
          shape.itemID = genID(state.shapeType);
          shape.extraData = { shapeType: state.shapeType };
          fabricCanvas.add(shape);
          curDrawingShape = shape;
        }
        curDrawingShape.points.push({ x: state.startPoint.x, y: state.startPoint.y });
      }
        break;
    }


  });

  fabricCanvas.on('mouse:move', function (e) {
    const shape = curDrawingShape;
    const startPoint = state.startPoint;
    const nextPoint = e.scenePoint;

    state.mouseX = e.scenePoint.x;
    state.mouseY = e.scenePoint.y;
    switch (state.shapeType) {

      case -2: {

        if (state.mousebuttonDown) {
          console.log('mouse:move', e)
          var delta = new fabric.Point(e.e.movementX, e.e.movementY);
          fabricCanvas.relativePan(delta);
        }

      }
        break;
      case OPTION_RECT: {//矩形
        // console.log('mouse:move', e)
        if (!state.mousebuttonDown) {
          return;
        }


        // 矩形参数计算（前面总结的4条公式）
        let top = Math.min(startPoint.y, nextPoint.y)
        let left = Math.min(startPoint.x, nextPoint.x)
        let width = Math.abs(startPoint.x - nextPoint.x)
        let height = Math.abs(startPoint.y - nextPoint.y)
        shape.top = top;
        shape.left = left;
        shape.width = width;
        shape.height = height;



      }
        break;
      case OPTION_PATH: { //折线

        if (!shape) {
          return;
        }

        if (!state.drawing) {
          return
        }
        shape.points[shape.points.length - 1].x = nextPoint.x;
        shape.points[shape.points.length - 1].y = nextPoint.y;

      }
        break;
      case OPTION_POLYGON: {
        if (!shape) {
          return;
        }


        if (!state.drawing) {
          return
        }

        shape.points[shape.points.length - 1].x = nextPoint.x;
        shape.points[shape.points.length - 1].y = nextPoint.y;

      }
        break;
    }

    fabricCanvas.requestRenderAll()

  });

  fabricCanvas.on('mouse:up', function (e) {

    // console.log('mouse:up', e.e.buttons, e)

    state.mousebuttonDown = false
    const shape = curDrawingShape;

    switch (state.shapeType) {
      case OPTION_POINT: //点
        if (!shape) {
          return
        }
        state.drawing = false;
        state.selectShapeId = shape.itemID;
        curDrawingShape = null
        break;
      case OPTION_RECT: { //rect
        state.drawing = false;

        if (!shape) {
          return
        }

        if (shape.width * shape.height <= 25) {

          state.fabricCanvas.remove(shape);
          curShape = null
          break;
        }


        const newShape = new fabric.Rect({
          fill: null,
          left: shape.left,
          top: shape.top,
          width: shape.width,
          height: shape.height,
          objectCaching: false,

          strokeWidth: 2, // 边框大小
          stroke: 'red', // 边框颜色

        });

        newShape.itemID = shape.itemID
        newShape.extraData = { ...shape.extraData }

        fabricCanvas.add(newShape);
        fabricCanvas.remove(shape);
        curDrawingShape = null;
        state.selectShapeId = shape.itemID;

      }
        break;
      case OPTION_PATH: { //折线
        if (!shape) {
          return
        }


        if (e.e.button === 2) {
          state.drawing = false;
          shape.points.pop()

          if (shape.points.length <= 1) {
            fabricCanvas.remove(shape);
            curDrawingShape = null;
            break;
          }

          const newShape = new fabric.Polyline([...shape.points], {
            strokeWidth: 2, // 边框大小
            stroke: 'red', // 边框颜色
            fill: null,
            hasControls: false, // 不显示控制器
            objectCaching: false,
            lockMovementY: true,
            lockMovementX: true,
            selectable: true,
          });

          newShape.itemID = shape.itemID
          newShape.extraData = { ...shape.extraData }

          fabricCanvas.add(newShape);
          fabricCanvas.remove(shape);
          curDrawingShape = null;
          state.selectShapeId = shape.itemID;
          break;
        }

      }
        break;
      case OPTION_POLYGON: {
        if (!shape) {
          return
        }

        if (e.e.button === 2) {

          state.drawing = false;
          shape.points.pop()

          if (shape.points.length <= 1) {
            fabricCanvas.remove(shape);
            curDrawingShape = null;
            break;
          }

          const newShape = new fabric.Polygon([...shape.points], {
            strokeWidth: 2, // 边框大小
            stroke: 'red', // 边框颜色
            fill: null,
            hasControls: false, // 不显示控制器
            objectCaching: false,
            // lockMovementY: true,
            // lockMovementX: true,
            selectable: true,

          });

          newShape.itemID = shape.itemID
          newShape.extraData = { ...shape.extraData }

          fabricCanvas.add(newShape);
          fabricCanvas.remove(shape);
          curDrawingShape = null;
          state.selectShapeId = shape.itemID;
          break
        }
      }
        break;
    }
    fabricCanvas.requestRenderAll();
  });



  state.fabricCanvas = fabricCanvas
}

const onFileChange = async (e) => {

  const filePath = await window.electronApi.openImageFile();

  // const file = event.target.files[0];



  // let imgPath = null // 获取图片文件真实路径 // 由于浏览器安全策略，现在需要这么做了 // 这段代码是网上复制下来的，想深入理解的可以百度搜搜 “C:\fakepath\”
  // if (window.createObjcectURL != undefined) { imgPath = window.createOjcectURL(filePath); }
  // else if (window.URL != undefined) { imgPath = window.URL.createObjectURL(filePath); }
  // else if (window.webkitURL != undefined) { imgPath = window.webkitURL.createObjectURL(filePath); }


  const image = await fabric.FabricImage.fromURL(filePath);
  state.fabricCanvas.backgroundImage = image;
  state.fabricCanvas.requestRenderAll();

}

const saveToFile = async (content) => {

  //exportToJSON
  await window.electronApi.exportToJSON(JSON.stringify(content));
  // const data = new Blob([content], { type: 'text/plain' });
  // const link = document.createElement('a');
  // link.href = URL.createObjectURL(data);
  // link.download = filename;
  // link.click();


}

const onSelectShapeId = (value) => {

  const shapes = state.fabricCanvas.getObjects();

  const shape = shapes.find(s => s.itemID === state.selectShapeId);

  const extraData = shape.extraData;

  state.selectShapeId = shape.itemID
  state.shapeData = { ...extraData };
  curShape = shape;
  state.fabricCanvas.setActiveObject(shape);
  state.fabricCanvas.requestRenderAll();

}

const onExportShapeToJSON = () => {

  const content = {
    shapes: [],
    pathPoints: [],
    pathLines: []
  }

  const elements = state.fabricCanvas.getObjects()

  for (const element of elements) {
    const o = element.toObject(['extraData'])
    const extraData = o.extraData;
    switch (extraData.shapeType) {
      case OPTION_POINT:
        {
          // const cp = element.getCenterPoint();
          // console.log(cp)
          content.shapes.push({
            type: 'circle',
            id: extraData.no,
            name: extraData.name,
            area: extraData.area,
            r: element.radius,
            cx: element.left,
            cy: element.top

          })
        }
        break;
      case OPTION_RECT:
        {

          content.shapes.push({
            type: 'rect',
            id: extraData.no,
            name: extraData.name,
            area: extraData.area,
            x: element.left,
            y: element.top,
            width: element.width,
            height: element.height,
            rotate: element.angle,
            scale: `${element.scaleX},${element.scaleY}`

          })
        }
        break;
      case OPTION_PATH:
        {

          content.pathLines.push({
            type: 'polyline',
            id: extraData.no,
            name: extraData.name,
            weight: extraData.weight,
            start: extraData.start,
            end: extraData.end,
            points: element.points.map(p => `${p.x},${p.y}`).join(' '),
          })

        }
        break;
      case OPTION_PATH_POINT:
        {

          content.pathPoints.push({
            type: 'circle',
            id: extraData.point.no,
            name: extraData.point.name,
            area: extraData.point.area,
            r: element.radius,
            cx: element.left,
            cy: element.top

          })
        }
        break;
      case OPTION_POLYGON: {
        content.shapes.push({
          type: 'polygon',
          id: extraData.no,
          name: extraData.name,
          area: extraData.area,
          points: element.points.map(p => `${p.x},${p.y}`).join(' '),
        })
      }
        break;
    }

  }

  // console.log(state.fabricCanvas.toDatalessJSON(['extraData']))
  // console.log(JSON.stringify(content))


  saveToFile(JSON.stringify(content))
  // const obj = state.fabricCanvas.getActiveObject()
  // state.shapJson = obj.toJSON(['extraData']);



}

const onSaveProject = async () => {



  const content = state.fabricCanvas.toDatalessJSON(['itemID', 'extraData'])

  await window.electronApi.saveProject(JSON.stringify(content));

}

const onLoadProject = async () => {
  const data = await window.electronApi.loadProject();
  console.log('onLoadProject', data)
  if (data) {
    state.shapeIdList = [];
    for (const obj of data.objects) {
      state.shapeIdList.push(obj.itemID);
    }
  }

  await state.fabricCanvas.loadFromJSON(data);
  state.fabricCanvas.requestRenderAll();

}


const save = async () => {
  curShape = getObjectByID(state.selectShapeId)

  if (!curShape) {
    ElMessage({
      message: '未选定图形',
      type: 'error',

    });
    return
  }

  switch (state.shapeData.shapeType) {
    case OPTION_PATH: {
      savePathShapeData()
    }
      break;
    default: {
      saveShapeData();

    }

  }
  ElMessage({
    message: '设置成功！',
    type: 'success',
  })
}
const saveShapeData = () => {
  curShape.extraData = { ...state.shapeData }

  // curShape = null;
}

const savePathShapeData = () => {
  //新点 就创建，已存在就更改起始点
  const radius = 4;
  const extraData = { ...state.shapeData };
  if (!state.pathPoints.find(item => item.no === extraData.start)) {

    const point = {
      no: state.pathPoints.length,
      name: extraData.start,
      type: extraData.startType,
      area: extraData.startArea,
      x: curShape.points[0].x,
      y: curShape.points[0].y

    }
    state.pathPoints.push(point);

    const shape = new fabric.Circle({
      fill: 'red',
      radius: radius,
      left: point.x,
      top: point.y,
      hasControls: false, // 不显示控制器
      // hasBorders: false, // 不显示控制器的边
      lockMovementY: true,
      lockMovementX: true,
      originX: 'center', // x轴方向以中心点为原点
      originY: 'center', // y轴方向以中心点为原点
      evented: false,

    });
    shape.itemID = genID(OPTION_PATH_POINT);
    shape.extraData = { shapeType: OPTION_PATH_POINT, point: { ...point } }
    state.fabricCanvas.add(shape)

    extraData.start = point.no;


  }
  else {
    const point = state.pathPoints.find(item => item.no === extraData.start);
    curShape.points[0].x = point.x;
    curShape.points[0].y = point.y;
  }


  if (!state.pathPoints.find(item => item.no === extraData.end)) {
    const point = {
      no: state.pathPoints.length,
      name: extraData.end,
      type: extraData.endType,
      area: extraData.endArea,
      x: curShape.points[curShape.points.length - 1].x,
      y: curShape.points[curShape.points.length - 1].y

    }
    state.pathPoints.push(point);

    const shape = new fabric.Circle({
      fill: 'red',
      radius: radius,
      left: point.x,
      top: point.y,
      hasControls: false, // 不显示控制器
      // hasBorders: false, // 不显示控制器的边
      lockMovementY: true,
      lockMovementX: true,
      originX: 'center', // x轴方向以中心点为原点
      originY: 'center', // y轴方向以中心点为原点
      evented: false,

    });
    shape.itemID = genID(OPTION_PATH_POINT);
    shape.extraData = { shapeType: OPTION_PATH_POINT, point: { ...point } }
    state.fabricCanvas.add(shape)

    extraData.end = point.no;

  }
  else {

    const point = state.pathPoints.find(item => item.no === extraData.end);
    curShape.points[curShape.points.length - 1].x = point.x;
    curShape.points[curShape.points.length - 1].y = point.y;

  }
  curShape.extraData = { ...extraData }

}

watch(() => state.shapeType, (nval, oval) => {
  curShape = null;

  switch (nval) {
    case -2:
    case OPTION_PATH:
    case OPTION_RECT:
    case OPTION_POINT:
      {
        state.fabricCanvas.selection = false;
      }
      break;
    case -1:
      {
        state.fabricCanvas.selection = true;
      }
      break;
  }
})
watch(() => state.selectShapeId, (nval, oval) => {


  const shapes = state.fabricCanvas.getObjects();

  const shape = shapes.find(s => s.itemID === state.selectShapeId);
  state.fabricCanvas.setActiveObject(shape);
  state.shapeData = { ...shape.extraData };
  curShape = shape;



})
const resizeObserver = new ResizeObserver(entries => {
  state.fabricCanvas.set('width', entries[0].contentRect.width);
  state.fabricCanvas.set('height', entries[0].contentRect.height)
  state.fabricCanvas.requestRenderAll();
});

onMounted(() => {

  resizeObserver.observe(drawerMain.value);


  document.onkeydown = function (event) {
    let e = event || window.event || arguments.callee.caller.arguments[0];
    if (e.target.nodeName !== 'INPUT' && e.target.nodeName !== "TEXTAREA" && e.keyCode === 46) {
      // console.log('delete', e)
      if (state.fabricCanvas) {
        state.fabricCanvas.remove(state.fabricCanvas.getActiveObject())
      }

    }
  }



  init();

})






</script>

<style lang="scss" scoped>
.drawer-container {
  height: 100vh;

}

.drawer-main {
  flex-grow: 1;
  flex-shrink: 1;

  box-sizing: border-box;
  margin-left: 80px;
  overflow: hidden;

  display: flex;


  canvas {
    flex-grow: 1;

    width: 100%;
    height: 100%;

    // background-color: red;


    // display: block;


  }



}

.drawer-toolbar {

  width: 32px;

  position: absolute;
  z-index: 999;



}
</style>
