# Pathfinding-robot-game
这个是基于unity开发的关于智能机器人寻路的项目

## 目的
建立一个包含若干个静止障碍物和运动障碍物的仿真环境，设定机器人的起始点和终点后，机器人能够规划出一条从起始点到目标点的安全路径。

- 一种基于C#内置随机生成方法与动态规划算法的具有若干静态动态障碍物的随机场景生成算法
-	基于简化人工势能法的局部动态路径规划算法的避障机器人
-	基于A*搜索算法的全局静态路径规划算法的避障机器人
-	将以上两种算法结合作动态路径规划算法的避障机器人

## 基本界面
### 设置界面
<image src="https://user-images.githubusercontent.com/78332649/215316888-9dc2d600-ef5e-4c63-875b-4d7b843932a7.png" width="50%"/>


### 随机场景

灰色为静态障碍物，米色为动态障碍物，棕色为边界，红色为目标点，球状物体为机器人

- 边长为10的场景
<image src="https://user-images.githubusercontent.com/78332649/215316930-23f6c09f-0acb-41e2-a9d3-704427d3187d.png" width="30%"/>

- 边长为20的场景
<image src="https://user-images.githubusercontent.com/78332649/215316940-9e60fd59-74e6-4c5c-9e62-e922aeab0542.png" width="30%"/>

### 寻路路线规划进度页面
为了防止用户以为程序卡顿，在寻路算法计算过程中， 我们设置了进度条表示当前的计算进度，让用户耐心等待
<image src="https://user-images.githubusercontent.com/78332649/215317684-8a90f6f0-3ff3-4cfe-ab1f-cf0a41d14b98.png" width="50%"/>


### 寻路场景
<image src="https://user-images.githubusercontent.com/78332649/215317200-ed2e529a-f586-4722-a747-7162fe7fe109.png" width="40%"/>

### 到达目的地提示场景
<image src="https://user-images.githubusercontent.com/78332649/215317768-098846ca-5824-43d9-9c83-9a58e40eea4f.png" width="50%"/>

## 基本操作
### 切换相机视角
使用空格键切换相机视角， 这里有两个视角，一个是跟随机器人视角，另一个是俯视视角
<image src="https://user-images.githubusercontent.com/78332649/215317367-0c007c9d-0acc-4a34-8778-ce31a4a00901.png" width="50%"/>
<image src="https://user-images.githubusercontent.com/78332649/215317376-4669615a-e29f-45bc-a766-ed8b68f8314f.png" width="50%"/>



