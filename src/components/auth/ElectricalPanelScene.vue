<template>
  <div ref="hostRef" class="panel-scene" aria-label="3D electrical control cabinet" role="img">
    <canvas ref="canvasRef"></canvas>
    <div class="scene-caption">
      <span>Pİ OTOMASYON</span>
      <strong>QR, dosya ve yetki yönetimi tek yerde</strong>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onBeforeUnmount, onMounted, ref } from 'vue';
import * as THREE from 'three';

const hostRef = ref<HTMLDivElement | null>(null);
const canvasRef = ref<HTMLCanvasElement | null>(null);

let renderer: THREE.WebGLRenderer | null = null;
let scene: THREE.Scene | null = null;
let camera: THREE.PerspectiveCamera | null = null;
let cabinetGroup: THREE.Group | null = null;
let resizeObserver: ResizeObserver | null = null;
let animationFrame = 0;
let pointerX = 0;
let pointerY = 0;

const materials = {
  cabinet: new THREE.MeshStandardMaterial({ color: 0xf8fafc, roughness: 0.58, metalness: 0.08 }),
  inner: new THREE.MeshStandardMaterial({ color: 0xe2e8f0, roughness: 0.72, metalness: 0.05 }),
  edge: new THREE.MeshStandardMaterial({ color: 0x475569, roughness: 0.52, metalness: 0.35 }),
  shadow: new THREE.MeshStandardMaterial({ color: 0x1e293b, roughness: 0.7, metalness: 0.1 }),
  rail: new THREE.MeshStandardMaterial({ color: 0x94a3b8, roughness: 0.42, metalness: 0.55 }),
  breaker: new THREE.MeshStandardMaterial({ color: 0xcbd5e1, roughness: 0.62, metalness: 0.04 }),
  darkBreaker: new THREE.MeshStandardMaterial({ color: 0x334155, roughness: 0.55, metalness: 0.08 }),
  copper: new THREE.MeshStandardMaterial({ color: 0xf59e0b, roughness: 0.32, metalness: 0.42 }),
  blue: new THREE.MeshStandardMaterial({ color: 0x2563eb, roughness: 0.45, metalness: 0.04 }),
  red: new THREE.MeshStandardMaterial({ color: 0xdc2626, roughness: 0.45, metalness: 0.04 }),
  green: new THREE.MeshStandardMaterial({ color: 0x16a34a, roughness: 0.45, metalness: 0.04 }),
  yellow: new THREE.MeshStandardMaterial({ color: 0xfacc15, roughness: 0.45, metalness: 0.04 }),
};

onMounted(() => {
  if (!hostRef.value || !canvasRef.value) return;

  scene = new THREE.Scene();
  scene.background = new THREE.Color(0xf5f7fb);

  camera = new THREE.PerspectiveCamera(32, 1, 0.1, 100);
  camera.position.set(0.05, 0.2, 12.8);

  renderer = new THREE.WebGLRenderer({
    canvas: canvasRef.value,
    antialias: true,
    alpha: false,
  });
  renderer.setPixelRatio(Math.min(window.devicePixelRatio, 2));
  renderer.shadowMap.enabled = true;
  renderer.shadowMap.type = THREE.PCFSoftShadowMap;

  scene.add(new THREE.AmbientLight(0xffffff, 1.6));

  const keyLight = new THREE.DirectionalLight(0xffffff, 2.2);
  keyLight.position.set(3.6, 5, 4);
  keyLight.castShadow = true;
  keyLight.shadow.mapSize.set(1024, 1024);
  scene.add(keyLight);

  const rimLight = new THREE.DirectionalLight(0x93c5fd, 1.1);
  rimLight.position.set(-4, 2.8, 3);
  scene.add(rimLight);

  const floor = new THREE.Mesh(
    new THREE.PlaneGeometry(12, 8),
    new THREE.ShadowMaterial({ color: 0x64748b, opacity: 0.16 }),
  );
  floor.rotation.x = -Math.PI / 2;
  floor.position.y = -2.06;
  floor.receiveShadow = true;
  scene.add(floor);

  cabinetGroup = buildCabinet();
  cabinetGroup.scale.setScalar(0.68);
  cabinetGroup.position.set(-0.06, 0.28, 0);
  scene.add(cabinetGroup);

  resizeObserver = new ResizeObserver(resizeScene);
  resizeObserver.observe(hostRef.value);
  resizeScene();

  hostRef.value.addEventListener('pointermove', handlePointerMove);
  animate();
});

onBeforeUnmount(() => {
  cancelAnimationFrame(animationFrame);
  resizeObserver?.disconnect();
  hostRef.value?.removeEventListener('pointermove', handlePointerMove);
  renderer?.dispose();
  cabinetGroup?.traverse((object) => {
    if (object instanceof THREE.Mesh) {
      object.geometry.dispose();
    }
  });
});

function buildCabinet(): THREE.Group {
  const group = new THREE.Group();
  group.rotation.y = -0.2;
  group.rotation.x = 0.04;

  const back = box(3.9, 4.2, 0.28, materials.inner, [-1.25, 0, -0.1]);
  back.castShadow = true;
  back.receiveShadow = true;
  group.add(back);

  const framePieces = [
    box(4.25, 0.16, 0.36, materials.cabinet, [0, 2.18, 0.02]),
    box(4.25, 0.16, 0.36, materials.cabinet, [0, -2.18, 0.02]),
    box(0.16, 4.45, 0.36, materials.cabinet, [0, 0, 0.02]),
    box(0.16, 4.45, 0.36, materials.cabinet, [0, 0, 0.02]),
  ];
  framePieces.forEach((piece) => group.add(piece));

  addRails(group);
  addModules(group);
  addTerminalBlocks(group);
  addWires(group);
  addDoor(group);

  return group;
}

function addRails(group: THREE.Group): void {
  [-1.15, 0.15, 1.35].forEach((y) => {
    const rail = box(3.45, 0.06, 0.08, materials.rail, [-1.25, y, 0.18]);
    rail.castShadow = true;
    group.add(rail);
  });
}

function addModules(group: THREE.Group): void {
  for (let row = 0; row < 3; row += 1) {
    const y = 1.42 - row * 1.25;
    for (let index = 0; index < 8; index += 1) {
      const width = index % 3 === 0 ? 0.34 : 0.25;
      const height = row === 1 && index > 4 ? 0.7 : 0.48;
      const x = -2.78 + index * 0.42;
      const material = row === 2 && index === 4 ? materials.darkBreaker : materials.breaker;
      const module = box(width, height, 0.28, material, [x, y, 0.42]);
      module.castShadow = true;
      group.add(module);

      const indicator = box(width * 0.55, 0.035, 0.02, index % 2 ? materials.green : materials.blue, [x, y + height * 0.28, 0.57]);
      group.add(indicator);
    }
  }

  const controller = box(0.82, 0.62, 0.34, materials.darkBreaker, [-1.5, -1.45, 0.5]);
  controller.castShadow = true;
  group.add(controller);

  const screen = box(0.52, 0.2, 0.02, materials.blue, [-1.5, -1.38, 0.68]);
  group.add(screen);
}

function addTerminalBlocks(group: THREE.Group): void {
  for (let index = 0; index < 14; index += 1) {
    const x = -2.9 + index * 0.23;
    const terminal = box(0.14, 0.24, 0.18, index % 2 ? materials.copper : materials.cabinet, [x, -1.88, 0.46]);
    terminal.castShadow = true;
    group.add(terminal);
  }
}

function addWires(group: THREE.Group): void {
  const colors = [materials.red, materials.blue, materials.yellow, materials.green];

  for (let index = 0; index < 18; index += 1) {
    const material = colors[index % colors.length];
    const startX = -2.85 + index * 0.19;
    const endX = startX + (index % 2 ? 0.26 : -0.18);
    const curve = new THREE.CatmullRomCurve3([
      new THREE.Vector3(startX, -1.75, 0.58),
      new THREE.Vector3(startX + 0.04, -1.36, 0.78),
      new THREE.Vector3(endX, -1.05, 0.58),
    ]);
    const wire = new THREE.Mesh(new THREE.TubeGeometry(curve, 16, 0.014, 8, false), material);
    wire.castShadow = true;
    group.add(wire);
  }
}

function addDoor(group: THREE.Group): void {
  const door = new THREE.Group();
  door.position.set(1.02, 0, 0.02);
  door.rotation.y = -1.02;

  const slab = box(2.85, 4.18, 0.18, materials.cabinet, [1.4, 0, 0]);
  slab.castShadow = true;
  slab.receiveShadow = true;
  door.add(slab);

  const innerPanel = box(2.55, 3.82, 0.035, materials.inner, [1.4, 0, 0.11]);
  door.add(innerPanel);

  const trimTop = box(2.72, 0.055, 0.06, materials.edge, [1.4, 2, 0.15]);
  const trimBottom = box(2.72, 0.055, 0.06, materials.edge, [1.4, -2, 0.15]);
  const trimLeft = box(0.055, 3.95, 0.06, materials.edge, [0.03, 0, 0.15]);
  const trimRight = box(0.055, 3.95, 0.06, materials.edge, [2.77, 0, 0.15]);
  door.add(trimTop, trimBottom, trimLeft, trimRight);

  const handle = box(0.16, 0.58, 0.12, materials.shadow, [2.52, -0.15, 0.28]);
  handle.castShadow = true;
  door.add(handle);

  const latch = box(0.22, 0.12, 0.14, materials.edge, [2.52, 0.28, 0.3]);
  door.add(latch);

  group.add(door);
}

function box(
  width: number,
  height: number,
  depth: number,
  material: THREE.Material,
  position: [number, number, number],
): THREE.Mesh {
  const mesh = new THREE.Mesh(new THREE.BoxGeometry(width, height, depth), material);
  mesh.position.set(...position);
  mesh.castShadow = true;
  mesh.receiveShadow = true;
  return mesh;
}

function handlePointerMove(event: PointerEvent): void {
  if (!hostRef.value) return;
  const bounds = hostRef.value.getBoundingClientRect();
  pointerX = ((event.clientX - bounds.left) / bounds.width - 0.5) * 2;
  pointerY = ((event.clientY - bounds.top) / bounds.height - 0.5) * 2;
}

function resizeScene(): void {
  if (!canvasRef.value || !renderer || !camera) return;
  const { width, height } = canvasRef.value.getBoundingClientRect();
  renderer.setSize(width, height, false);
  camera.aspect = width / Math.max(height, 1);
  camera.updateProjectionMatrix();
}

function animate(): void {
  animationFrame = requestAnimationFrame(animate);
  if (!renderer || !scene || !camera || !cabinetGroup) return;

  const time = performance.now() * 0.001;
  cabinetGroup.rotation.y += ((-0.2 + pointerX * 0.08) - cabinetGroup.rotation.y) * 0.04;
  cabinetGroup.rotation.x += ((0.04 - pointerY * 0.04) - cabinetGroup.rotation.x) * 0.04;
  cabinetGroup.position.y = 0.18 + Math.sin(time * 1.25) * 0.025;

  renderer.render(scene, camera);
}

</script>

<style scoped>
.panel-scene {
  position: relative;
  min-height: 26rem;
  height: 100%;
  overflow: hidden;
  border-radius: 8px;
  background:
    radial-gradient(circle at 30% 18%, rgba(37, 99, 235, 0.14), transparent 28%),
    linear-gradient(145deg, #f8fafc 0%, #eef2f7 58%, #e5eaf2 100%);
  border: 1px solid rgba(148, 163, 184, 0.34);
}

.panel-scene canvas {
  width: 100%;
  height: calc(100% - 5.1rem);
  min-height: 20rem;
  display: block;
}

.scene-caption {
  position: absolute;
  left: 1rem;
  right: 1rem;
  bottom: 1rem;
  display: grid;
  gap: 0.25rem;
  padding: 0.85rem 0.95rem;
  border: 1px solid rgba(203, 213, 225, 0.68);
  border-radius: 8px;
  background: rgba(255, 255, 255, 0.82);
  backdrop-filter: blur(12px);
  box-shadow: 0 14px 38px rgba(15, 23, 42, 0.1);
}

.scene-caption span {
  color: var(--app-primary);
  font-size: 0.72rem;
  font-weight: 800;
  text-transform: uppercase;
}

.scene-caption strong {
  color: var(--app-text);
  font-size: 0.95rem;
  line-height: 1.25;
}

@media (max-width: 760px) {
  .panel-scene {
    min-height: 18.5rem;
  }

  .panel-scene canvas {
    min-height: 13.5rem;
    height: calc(100% - 4.65rem);
  }

  .scene-caption {
    padding: 0.7rem 0.8rem;
  }

  .scene-caption strong {
    font-size: 0.85rem;
  }
}
</style>
