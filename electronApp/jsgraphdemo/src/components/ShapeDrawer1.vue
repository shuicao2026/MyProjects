<template>
  <div class="drawer-container">
    <div class="drawer-toolbar">
      <el-radio-group v-model="state.shapeType">
        <el-radio-button label="移动" :value="-2" />
        <el-radio-button label="选择" :value="-1" />
        <el-radio-button label="点位" :value="OPTION_POINT" />
        <el-radio-button label="矩形" :value="OPTION_RECT" />
        <el-radio-button label="多边形" :value="OPTION_POLYGON" />
        <el-radio-button label="路径" :value="OPTION_PATH" />
      </el-radio-group>

      <el-button @click="onExportShapeToJSON">导出JSON</el-button>
      <input type="file" accept="image/*" @change="(e) => onFileChange(e)" />
      <div>{{ state.shapJson }}</div>

      <div>{{ state.mouseX }},{{ state.mouseY }}</div>
    </div>

    <div class="drawer-right">

    </div>

    <div class="drawer-main" id="drawer-main" ref="drawerMain">
      <canvas id="drawer" ref="drawer"
        style="box-shadow: rgba(60, 64, 67, 0.3) 0px 1px 2px 0px, rgba(60, 64, 67, 0.15) 0px 1px 3px 1px;">


      </canvas>
    </div>


    <el-drawer v-model="state.shapeDialogShow" :modal="false" :title="dialogTitle" size="320">
      <el-form label-width="auto" style="max-width: 600px">
        <el-form-item label="编号">
          <el-input v-model="state.shapeData.no" />
        </el-form-item>
        <el-form-item label="名称">
          <el-input v-model="state.shapeData.name" />
        </el-form-item>
      </el-form>
      <template #footer>
        <div class="dialog-footer">
          <el-button @click="state.shapeDialogShow = false">取消</el-button>
          <el-button type="primary" @click="saveShapeData">
            设置
          </el-button>
        </div>
      </template>
    </el-drawer>


    <el-drawer v-model="state.pathDialogShow" :modal="false" :title="dialogTitle" size="320">
      <el-form label-width="auto" style="max-width: 600px">
        <el-form-item label="编号">
          <el-input v-model="state.shapeData.no" />
        </el-form-item>
        <el-form-item label="名称">
          <el-input v-model="state.shapeData.name" />
        </el-form-item>
        <el-form-item label="权重">
          <el-input-number v-model="state.shapeData.weight" :min="0" />
        </el-form-item>
        <el-form-item label="起始点">
          <el-row :gutter="4">
            <el-col :span="12">
              <el-select v-model="state.shapeData.start" filterable allow-create default-first-option @change="() => {
                const item = state.pathPoints[state.shapeData.start]//.find(item => item.no === extraData.start);

                state.shapeData.startType = item?.type
              }">
                <el-option v-for="item in state.pathPoints" :key="item.no" :label="item.name" :value="item.no" />
              </el-select>
            </el-col>
            <el-col :span="12">
              <el-radio-group v-model="state.shapeData.startType">
                <el-radio-button label="店门口" :value="0" size="small" />
                <el-radio-button label="楼梯口" :value="1" size="small" />
                <el-radio-button label="出入口" :value="2" size="small" />
              </el-radio-group>
            </el-col>
          </el-row>



        </el-form-item>
        <el-form-item label="结束点">

          <el-row :gutter="4">
            <el-col :span="12">
              <el-select v-model="state.shapeData.end" filterable allow-create default-first-option @change="() => {
                const item = state.pathPoints[state.shapeData.end]//.find(item => item.no === extraData.end);

                state.shapeData.endType = item?.type
              }">
                <el-option v-for="item in state.pathPoints" :key="item.no" :label="item.name" :value="item.no" />
              </el-select>
            </el-col>
            <el-col :span="12">
              <el-radio-group v-model="state.shapeData.endType">
                <el-radio-button label="店门口" :value="0" size="small" />
                <el-radio-button label="楼梯口" :value="1" size="small" />
                <el-radio-button label="出入口" :value="2" size="small" />
              </el-radio-group>
            </el-col>
          </el-row>
        </el-form-item>

      </el-form>
      <template #footer>
        <div class="dialog-footer">
          <el-button @click="state.pathDialogShow = false">取消</el-button>
          <el-button type="primary" @click="savePathShapeData">
            设置
          </el-button>
        </div>
      </template>
    </el-drawer>


  </div>


</template>

<script setup>
import { nanoid } from 'nanoid'
import * as fabric from 'fabric'

import { computed, onMounted, reactive, ref, watch } from 'vue';


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


  shapeType: -2, //0 点  1  矩形 2 路径
  currShape: null,
  pathDialogShow: false,
  shapeDialogShow: false,
  shapeData: {
    no: null,
    name: null
  },
  pathPoints: [],

});

const dialogTitle = computed(() => {

  if (!state.currShape || !state.currShape.extraData) {
    return '';
  }


  switch (state.currShape.extraData.shapeType) {

    case OPTION_POINT:
      return '点位信息';
    case OPTION_RECT:
      return '矩形信息';
    case OPTION_POLYGON:
      return '多边形信息';
    case OPTION_PATH:
      return '路径信息';
  }


  return '';


})

const genID = (shapeType) => {

  switch (shapeType) {

    case OPTION_POINT:
      return 'POINT_' + nanoid(10);
    case OPTION_RECT:
      return 'RECT_' + nanoid(10);
    case OPTION_POLYGON:
      return 'POLYGON_' + nanoid(10);
    case OPTION_PATH:
      return 'PATH_' + nanoid(10);
    case OPTION_PATH_POINT:
      return 'PATH_POINT_' + nanoid(10);

  }
  return null

}

const init = async () => {
  const fabricCanvas = new fabric.Canvas(drawer.value, {
    backgroundColor: "#fff",
    width: 1280,
    height: 800,
    altSelectionKey: 'altKey',

    selection: false,
    fireRightClick: true, // 启用右键，button的数字为3
    stopContextMenu: true, // 禁止默认右键菜单,
    selectionKey: 'ctrlKey',
    originX: 'left',
    originY: 'top'
  });

  //mouse:dblclick
  fabricCanvas.on("mouse:dblclick", function (e) {

    if (!e.target) {
      return
    }


    const shape = e.target;
    // alert(shape.type)
    const extraData = shape.extraData;
    switch (extraData.shapeType) {

      case OPTION_PATH:
        {//路径
          state.shapeData = { ...extraData };
          state.currShape = shape;
          state.pathDialogShow = true;


        }
        break;
      default:
        {
          state.shapeData = { ...extraData };
          state.currShape = shape;
          state.shapeDialogShow = true;

        }
    }
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

    state.startPoint = e.scenePoint
    state.mousebuttonDown = true;
    switch (state.shapeType) {

      case OPTION_POINT: {
        if (e.target) {
          return;
        }
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
        state.currShape = shape;
      }
        break;
      case OPTION_RECT: {//

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
        state.currShape = shape;
      }
        break;
      case OPTION_PATH: { //折线

        if (!state.currShape) {
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
          state.currShape = shape;
        }

        state.currShape.points.push({ x: state.startPoint.x, y: state.startPoint.y })
      }
        break;
      case OPTION_POLYGON: {

        if (!state.currShape) {
          const shape = new fabric.Polygon([{ x: state.startPoint.x, y: state.startPoint.y }], {
            strokeWidth: 2, // 边框大小
            stroke: 'red', // 边框颜色
            fill: null,
            hasControls: false, // 不显示控制器
            objectCaching: false,
            lockMovementY: true,
            lockMovementX: true,
            selectable: true,
          });
          shape.itemID = genID(state.shapeType);
          shape.extraData = { shapeType: state.shapeType };
          fabricCanvas.add(shape);
          state.currShape = shape;
        }
        state.currShape.points.push({ x: state.startPoint.x, y: state.startPoint.y })
      }
        break;
    }


  });

  fabricCanvas.on('mouse:move', function (e) {
    const shape = state.currShape;
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
        console.log('mouse:move', e)
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

        // if (!shape) {
        //   return;
        // }

        shape.points[shape.points.length - 1].x = nextPoint.x;
        shape.points[shape.points.length - 1].y = nextPoint.y;

      }
        break;
      case OPTION_POLYGON: {
        if (!shape) {
          return;
        }

        shape.points[shape.points.length - 1].x = nextPoint.x;
        shape.points[shape.points.length - 1].y = nextPoint.y;

      }
        break;
    }

    fabricCanvas.requestRenderAll()

  });

  fabricCanvas.on('mouse:up', function (e) {

    console.log('mouse:up', e.e.buttons, e)

    state.mousebuttonDown = false
    const shape = state.currShape;

    switch (state.shapeType) {
      case OPTION_POINT: //点

        break;
      case OPTION_RECT: { //rect

        if (state.currShape.width * state.currShape.height <= 25) {

          state.fabricCanvas.remove(state.currShape);
          state.currShape = null
        }

      }
        break;
      case OPTION_PATH: { //折线
        if (!shape) {
          return
        }


        if (e.e.button === 2) {
          state.currShape = null;
          shape.points.pop()
          break;
        }
        // const points = shape.points
        // points.push({ x: nextPoint.x, y: nextPoint.y })

      }
        break;
      case OPTION_POLYGON: {
        if (!shape) {
          return
        }
        if (e.e.button === 2) {
          state.currShape = null;
          shape.points.pop()
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

  const file = event.target.files[0];


  let imgPath = null // 获取图片文件真实路径 // 由于浏览器安全策略，现在需要这么做了 // 这段代码是网上复制下来的，想深入理解的可以百度搜搜 “C:\fakepath\”
  if (window.createObjcectURL != undefined) { imgPath = window.createOjcectURL(file); }
  else if (window.URL != undefined) { imgPath = window.URL.createObjectURL(file); }
  else if (window.webkitURL != undefined) { imgPath = window.webkitURL.createObjectURL(file); }


  const image = await fabric.FabricImage.fromURL(imgPath);
  state.fabricCanvas.backgroundImage = image;
  state.fabricCanvas.requestRenderAll();

}

const saveToFile = (content, filename) => {

  const data = new Blob([content], { type: 'text/plain' });
  const link = document.createElement('a');
  link.href = URL.createObjectURL(data);
  link.download = filename;
  link.click();


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
          const cp = element.getCenterPoint();
          // console.log(cp)
          content.shapes.push({
            type: 'circle',
            id: extraData.no,
            name: extraData.name,
            r: element.radius,
            cx: cp.x,
            cy: cp.y

          })
        }
        break;
      case OPTION_RECT:
        {

          content.shapes.push({
            type: 'rect',
            id: extraData.no,
            name: extraData.name,
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
          const cp = element.getCenterPoint();
          content.pathPoints.push({
            type: 'circle',
            id: extraData.point.no,
            name: extraData.point.name,
            r: element.radius,
            cx: cp.x,
            cy: cp.y

          })
        }
        break;
      case OPTION_POLYGON: {
        content.pathLines.push({
          type: 'polygon',
          id: extraData.no,
          name: extraData.name,
          points: element.points.map(p => `${p.x},${p.y}`).join(' '),
        })
      }
        break;
    }

  }

  // console.log(state.fabricCanvas.toDatalessJSON(['extraData']))
  // console.log(JSON.stringify(content))


  saveToFile(JSON.stringify(content), 'shapes.json')
  // const obj = state.fabricCanvas.getActiveObject()
  // state.shapJson = obj.toJSON(['extraData']);



}

const saveShapeData = () => {
  state.currShape.extraData = { ...state.shapeData }

  state.shapeDialogShow = false;
  state.currShape = null;
}

const savePathShapeData = () => {
  //新点 就创建，已存在就更改起始点
  const radius = 4;
  const extraData = { ...state.shapeData };
  console.log(extraData)
  state.pathDialogShow = false;
  if (!state.pathPoints.find(item => item.no === extraData.start)) {

    const point = {
      no: state.pathPoints.length,
      name: extraData.start,
      type: extraData.startType,
      x: state.currShape.points[0].x,
      y: state.currShape.points[0].y

    }
    state.pathPoints.push(point);

    const shape = new fabric.Circle({
      fill: 'red',
      radius: radius,
      left: point.x - radius,
      top: point.y - radius,
      hasControls: false, // 不显示控制器
      hasBorders: false, // 不显示控制器的边
      lockMovementY: true,
      lockMovementX: true,
      evented: false,

    });
    shape.itemID = genID(OPTION_PATH_POINT);
    shape.extraData = { shapeType: OPTION_PATH_POINT, point: { ...point } }
    state.fabricCanvas.add(shape)

    extraData.start = point.no;


  }
  else {
    const point = state.pathPoints.find(item => item.no === extraData.start);
    state.currShape.points[0].x = point.x;
    state.currShape.points[0].y = point.y;
  }


  if (!state.pathPoints.find(item => item.no === extraData.end)) {
    const point = {
      no: state.pathPoints.length,
      name: extraData.end,
      type: extraData.endType,
      x: state.currShape.points[state.currShape.points.length - 1].x,
      y: state.currShape.points[state.currShape.points.length - 1].y

    }
    state.pathPoints.push(point);

    const shape = new fabric.Circle({
      fill: 'red',
      radius: radius,
      left: point.x - radius,
      top: point.y - radius,
      hasControls: false, // 不显示控制器
      hasBorders: false, // 不显示控制器的边
      lockMovementY: true,
      lockMovementX: true,
      evented: false,

    });
    shape.itemID = genID(OPTION_PATH_POINT);
    shape.extraData = { shapeType: OPTION_PATH_POINT, point: { ...point } }
    state.fabricCanvas.add(shape)

    extraData.end = point.no;

  }
  else {

    const point = state.pathPoints.find(item => item.no === extraData.end);
    state.currShape.points[state.currShape.points.length - 1].x = point.x;
    state.currShape.points[state.currShape.points.length - 1].y = point.y;

  }
  state.currShape.extraData = { ...extraData }

}

watch(() => state.shapeType, (nval, oval) => {
  state.currShape = null;

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


onMounted(() => {
  init();

  document.onkeydown = function (event) {
    let e = event || window.event || arguments.callee.caller.arguments[0];
    if (e.target.nodeName !== 'INPUT' && e.target.nodeName !== "TEXTAREA" && e.keyCode === 46) {
      // console.log('delete', e)
      if (state.fabricCanvas) {
        state.fabricCanvas.remove(state.fabricCanvas.getActiveObject())
      }

    }
  }


})






</script>

<style lang="scss" scoped>
.drawer-container {

  margin: 0;
  padding: 0;
  height: 100%;
  width: 100%;
  position: relative;

  // background-color: red;
  display: flex;
  justify-content: stretch;
  align-items: stretch;
  // flex-direction: column;

  .drawer-toolbar {
    // flex-shrink: 1;
    // flex-grow: 0;
    position: absolute;
    z-index: 999;

    width: 64px;

  }

  .drawer-right {
    flex-shrink: 1;
    flex-grow: 0;
  }

  .drawer-main {

    box-sizing: border-box;
    background-color: rgba(230, 227, 227, 0.8);

    display: flex;
    justify-content: center;


    padding: 10px;

    // box-shadow: rgba(60, 64, 67, 0.3) 0px 1px 2px 0px, rgba(60, 64, 67, 0.15) 0px 1px 3px 1px;
  }
}
</style>
