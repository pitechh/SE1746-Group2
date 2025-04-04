const routes = [
  {
    name: "Login",
    path: "/login",
    component: () => import("pages/Login.vue"),
  },
  {
    path: "/",
    component: () => import("layouts/MainLayout.vue"),
    meta: { requireLogin: true },
    children: [
      {
        name: "Salary Formula Config",
        path: "/salary-formula-config",
        component: () => import("src/pages/SalaryFormulaConfig/SalaryFormulaConfig.vue"),
      },
      {
        name: "List Employee KT",
        path: "/list-employee-kt",
        component: () => import("src/pages/Employee/ListEmployeeForKT.vue"),
      },
      {
        name: "List Employee QTNS",
        path: "/list-employee-qtns",
        component: () => import("src/pages/Employee/ListEmployeeForQTNS.vue"),
      },
      {
        name: "List Employee",
        path: "/list-employee",
        component: () => import("src/pages/Employee/ListEmployee.vue"),
      },
      {
        name: "Update Personal Information",
        path: "/self-update",
        component: () => import("pages/Account/UpdateAccountInformation.vue"),
      },
      {
        name: "Change Password",
        path: "/change-password",
        component: () => import("pages/Account/ChangePassword.vue"),
      },
      {
        name: "Category",
        path: "/category",
        component: () => import("pages/Category/Category.vue"),
      },
      {
        name: "Position",
        path: "/position",
        component: () => import("pages/Position/Position.vue"),
      },
      {
        name: "Department",
        path: "/department",
        component: () => import("pages/Department/Department.vue"),
      },
      {
        name: "List Project",
        path: "/project",
        component: () => import("pages/Project/ListProject.vue"),
      },
      {
        name: "List View Project",
        path: "/project-view",
        component: () => import("pages/Project/ListViewProject.vue"),
      },
      {
        name: "List Account",
        path: "/list-account-admin",
        component: () => import("pages/Account/ListAccount.vue"),
      },
      {
        name: "List Account QTDA",
        path: "/list-account-qtda",
        component: () => import("pages/Account/ListAccountQTDA.vue"),
      },
    ],
  },

  // Always leave this as last one,
  // but you can also remove it
  {
    path: "/:catchAll(.*)*",
    component: () => import("pages/Error404.vue"),
  },
];

export default routes;
