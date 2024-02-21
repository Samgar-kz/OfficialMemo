import { createRouter, createWebHistory } from "vue-router";

const routes = [
  {
    path: "/",
    name: "CreatePage",
    component: () =>
      import(/* webpackChunkName: "CreatePage" */ "@/pages/CreatePage.vue"),
  },
  {
    path: "/:requestGuid/approve/:taskGuid",
    name: "ApprovalPage",
    component: () => import("@/pages/ApprovalPage.vue"),
  },
  {
    path: "/:requestGuid/assignment/:taskGuid",
    name: "AssigmentPerformPage",
    component: () => import("@/pages/AssigmentPerformPage.vue"),
  },
  {
    path: "/:requestGuid/assignment/:taskGuid/update",
    name: "AssigmentUpdatePage",
    component: () => import("@/pages/AssigmentUpdatePage.vue"),
  },
  {
    path: "/:requestGuid/assignment/:taskGuid/approve",
    name: "AssigmentApprovePage",
    component: () => import("@/pages/AssigmentApprovePage.vue"),
  },
  {
    path: "/:processGuid/view",
    name: "ViewPage",
    component: () => import("@/pages/ViewPage.vue"),
  },
  {
    path: "/:processGuid/view/:status",
    name: "ViewArchivePage",
    component: () => import("@/pages/ViewPage.vue"),
  },
  {
    path: "/:requestGuid/sign",
    name: "SigningPage",
    component: () => import("@/pages/SigningPage.vue"),
  },
  {
    path: "/:requestGuid/rework",
    name: "ReworkPage",
    component: () => import("@/pages/ReworkPage.vue"),
  },
  {
    path: "/:processGuid/edit",
    name: "EditPage",
    component: () => import("@/pages/EditPage.vue"),
  },
  {
    path: "/:requestGuid/review",
    name: "ReviewPage",
    component: () => import("@/pages/ReviewPage.vue"),
  },
  {
    path: "/:requestGuid/perform/:taskGuid",
    name: "PerformPage",
    component: () => import("@/pages/PerformPage.vue"),
  },
  {
    path: "/:requestGuid/check",
    name: "CheckPage",
    component: () => import("@/pages/CheckPage.vue"),
  },
];

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
});

export default router;
