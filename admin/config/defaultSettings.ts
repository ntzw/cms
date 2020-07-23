import { Settings as ProSettings } from '@ant-design/pro-layout';

type DefaultSettings = ProSettings & {
  pwa: boolean;
  basePath: string;
};

let basePath = '';
const { NODE_ENV } = process.env;
if (NODE_ENV === 'development') {
  basePath = 'https://localhost:5005';
}

const proSettings: DefaultSettings = {
  navTheme: 'dark',
  // 拂晓蓝
  primaryColor: '#1890ff',
  layout: 'topmenu',
  contentWidth: 'Fluid',
  fixedHeader: false,
  fixSiderbar: true,
  colorWeak: false,
  menu: {
    locale: true,
  },
  title: 'Ant Design Pro',
  pwa: false,
  iconfontUrl: '',
  basePath,
};

export type { DefaultSettings };

export default proSettings;

