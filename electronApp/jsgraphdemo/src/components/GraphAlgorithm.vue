<template>
    <div>
        {{ state.message }}
    </div>
</template>

<script setup>
import { onMounted, reactive } from 'vue';

const state = reactive({
    message: null
});

/**
 *          a 8    g
 *         /  5  /   9 
 *        b  -  c  3    f  7  h    
 *       /       10   /
 *      d          e
 */
// 0 1 2 3 4 5 6 7
// a b c d e f g h
const vex = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h'];

const matrix = [
    //a  b  c  d         e         f         g  h
    [0, 10, 5, Infinity, Infinity, Infinity, 8, Infinity],//a
    [10, 0, 4, 3, Infinity, Infinity, Infinity, Infinity],//b
    [5, 4, 0, Infinity, 10, 3, 1, Infinity],//c
    [Infinity, 3, Infinity, 0, Infinity, Infinity, Infinity, Infinity],//d
    [Infinity, Infinity, 10, Infinity, 0, 15, Infinity, Infinity],//e
    [Infinity, Infinity, 3, Infinity, 15, 0, 9, 7],//f
    [8, Infinity, 1, Infinity, Infinity, 9, 0, Infinity],//g
    [Infinity, Infinity, Infinity, Infinity, Infinity, 7, Infinity, 0],//h
]


const visited = new Array(vex.length).fill(false);

const BFS = (v, vex, matrix, visited) => {
    // const visit = new Array(8).fill(false)
    const queue = [];
    visited[vex.indexOf(v)] = true;
    queue.push(vex.indexOf(v));
    while (queue.length > 0) {
        const index = queue.shift();
        state.message.push(vex[index])
        for (let i = 0; i < matrix[index].length; i++) {

            if (matrix[index][i] && !visited[i]) {
                console.log(i, vex[i], visited[i])
                visited[i] = true
                queue.push(i)
            }

        }


    }

}



const DFS = (v, vex, matrix, visited) => {


    const index = vex.indexOf(v)
    state.message.push(v)
    visited[index] = true;//访问完后，visited对应的下标标记为1

    for (let i = 0; i < matrix[index].length; i++) {
        if (matrix[index][i] && !visited[i]) {

            DFS(vex[i], vex, matrix, visited);

        }
    }


}





const Dijkstra = (src, dst, vex, matrix) => {

    const path = new Array(vex.length).fill(-1); //保存最短路径,采用前置节点表示
    const dist = new Array(vex.length).fill(Infinity); // 保存到其他点的距离

    const visited = new Array(vex.length).fill(false)
    const queue = [];

    const startIndex = vex.indexOf(src);
    dist[startIndex] = 0;
    queue.push(startIndex);


    while (queue.length > 0) {
        queue.sort((a, b) => dist[a] - dist[b]);//将距离最短的排在最前面
        const index = queue.shift();
        if (visited[index]) {
            continue;
        }
        visited[index] = true;

        for (let i = 0; i < matrix[index].length; i++) {

            if (matrix[index][i] != 0 && matrix[index][i] != Infinity && dist[i] > matrix[index][i] + dist[index]) {

                dist[i] = matrix[index][i] + dist[index];
                path[i] = index;
                queue.push(i);

            }
        }
    }

    const pathArry = [];
    const pathIndex = [];
    const dstIndex = vex.indexOf(dst);
    let j = dstIndex;
    while (j >= 0) {
        pathArry.push(vex[j])

        if (path[j] >= 0) {
            pathIndex.push([path[j], j])
        }

        j = path[j]
    }

    return {
        distance: dist[dstIndex],
        pathArry: pathArry.reverse(),
        pathIndex: pathIndex.reverse(),


    }

}



onMounted(() => {

    const a = Infinity

    state.message = Dijkstra('a', 'h', vex, matrix);
    //   DFS('a', vex, matrix, visited);

});




</script>

<style lang="scss" scoped></style>