// https://umijs.org/config/
import { defineConfig } from 'umi';
import defaultSettings from './defaultSettings';
import proxy from './proxy';

const { REACT_APP_ENV } = process.env;

export default defineConfig({
  hash: true,
  antd: {},
  dva: {
    hmr: true,
  },
  locale: {
    // default zh-CN
    default: 'zh-CN',
    // default true, when it is true, will use `navigator.language` overwrite default
    antd: true,
    baseNavigator: true,
  },
  dynamicImport: {
    loading: '@/components/PageLoading/index',
  },
  targets: {
    ie: 11,
  },
  // umi routes: https://umijs.org/docs/routing
  routes: [
    {
      path: '/user',
      component: '../layouts/UserLayout',
      routes: [
        {
          name: 'login',
          path: '/user/login',
          component: './user/login',
        },
      ],
    },
    {
      path: '/code',
      name: 'code',
      component: './CodeGenerate',
    },
    {
      path: '/',
      component: '../layouts/SecurityLayout',
      routes: [
        {
          path: '/',
          component: '../layouts/BasicLayout',
          //authority: ['admin', 'user'],
          routes: [
            {
              path: '/',
              redirect: '/welcome',
            },
            {
              path: '/welcome',
              name: 'welcome',
              icon: 'smile',
              component: './Welcome',
            },
            {
              path: '/account',
              name: 'admin',
              icon: 'crown',
              //component: './Admin',
              //authority: ['admin'],
              routes: [
                {
                  path: 'adminlist',
                  name: 'adminlist',
                  icon: 'user',
                  component: './account/adminlist',
                  //authority: ['admin'],
                },
                {
                  path: 'rolelist',
                  name: 'rolelist',
                  icon: 'user',
                  component: './account/rolelist',
                  //authority: ['admin'],
                },
              ],
            },
            {
              path: '/cms',
              name: 'cms',
              icon: 'crown',
              //component: './Admin',
              //authority: ['admin'],
              routes: [
                {
                  path: 'sitelist',
                  name: 'sitelist',
                  icon: 'user',
                  component: './cms/sitelist',
                  //authority: ['admin'],
                },
                {
                  path: 'modeltablelist',
                  name: 'modeltablelist',
                  icon: 'user',
                  component: './cms/modeltablelist',
                  //authority: ['admin'],
                },
                {
                  path: 'columnlist',
                  name: 'columnlist',
                  icon: 'user',
                  component: './cms/columnlist',
                  //authority: ['admin'],
                },
                {
                  path: 'content',
                  name: 'content',
                  icon: 'user',
                  component: './cms/content',
                  //authority: ['admin'],
                },
              ],
            },
            {
              name: 'list.table-list',
              icon: 'table',
              path: '/list',
              component: './ListTableList',
            },
            {
              component: './404',
            },
          ],
        },
        {
          component: './404',
        },
      ],
    },
    {
      component: './404',
    },
  ],
  // Theme for antd: https://ant.design/docs/react/customize-theme-cn
  theme: {
    // ...darkTheme,
    'primary-color': defaultSettings.primaryColor,
  },
  // @ts-ignore
  title: false,
  ignoreMomentLocale: true,
  proxy: proxy[REACT_APP_ENV || 'dev'],
  manifest: {
    basePath: '/',
  },
});
