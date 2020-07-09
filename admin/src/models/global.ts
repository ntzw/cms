import { Subscription, Reducer, Effect } from 'umi';

import { NoticeIconData } from '@/components/NoticeIcon';
import { queryNotices } from '@/services/user';
import { ConnectState } from './connect.d';
import { postAjax } from '@/services/global';
import { HandleResult } from '@/utils/request';

export interface NoticeItem extends NoticeIconData {
  id: string;
  type: string;
  status: string;
}

export interface SiteSelectItem {
  name: string;
  num: string;
  isDefault: boolean;
}

export interface GlobalModelState {
  collapsed: boolean;
  notices: NoticeItem[];
  siteData: SiteSelectItem[];
  selectedSite?: SiteSelectItem;
}

export interface GlobalModelType {
  namespace: 'global';
  state: GlobalModelState;
  effects: {
    fetchSites: Effect;
    fetchNotices: Effect;
    clearNotices: Effect;
    changeNoticeReadState: Effect;
    post: Effect;
  };
  reducers: {
    setCurrentSite: Reducer<GlobalModelState>;
    setSiteData: Reducer<GlobalModelState>;
    changeLayoutCollapsed: Reducer<GlobalModelState>;
    saveNotices: Reducer<GlobalModelState>;
    saveClearedNotices: Reducer<GlobalModelState>;
  };
  subscriptions: { setup: Subscription };
}

const GlobalModel: GlobalModelType = {
  namespace: 'global',

  state: {
    collapsed: false,
    notices: [],
    siteData: [],
  },

  effects: {
    *post({ url, data, callback }, { call }) {
      const res: HandleResult = yield call(postAjax, url, data);
      if (typeof callback === 'function')
        callback(res)
    },
    *fetchSites(_, { call, put, select }) {
      const res: HandleResult<SiteSelectItem[]> = yield call(postAjax, '/Api/CMS/Site/SelectData');
      if (res.isSuccess) {
        yield put({
          type: 'setSiteData',
          payload: res.data
        });

        const selectedSite: SiteSelectItem | undefined = yield select(
          (state: ConnectState) => state.global.selectedSite,
        );
        if (!selectedSite) {
          const defaultSite = res.data?.find(temp => temp.isDefault);
          yield put({
            type: 'setCurrentSite',
            payload: defaultSite || res.data?.[0],
          });
        }

      }
    },
    *fetchNotices(_, { call, put, select }) {
      const data = yield call(queryNotices);
      yield put({
        type: 'saveNotices',
        payload: data,
      });
      const unreadCount: number = yield select(
        (state: ConnectState) => state.global.notices.filter((item) => !item.read).length,
      );
      yield put({
        type: 'user/changeNotifyCount',
        payload: {
          totalCount: data.length,
          unreadCount,
        },
      });
    },
    *clearNotices({ payload }, { put, select }) {
      yield put({
        type: 'saveClearedNotices',
        payload,
      });
      const count: number = yield select((state: ConnectState) => state.global.notices.length);
      const unreadCount: number = yield select(
        (state: ConnectState) => state.global.notices.filter((item) => !item.read).length,
      );
      yield put({
        type: 'user/changeNotifyCount',
        payload: {
          totalCount: count,
          unreadCount,
        },
      });
    },
    *changeNoticeReadState({ payload }, { put, select }) {
      const notices: NoticeItem[] = yield select((state: ConnectState) =>
        state.global.notices.map((item) => {
          const notice = { ...item };
          if (notice.id === payload) {
            notice.read = true;
          }
          return notice;
        }),
      );

      yield put({
        type: 'saveNotices',
        payload: notices,
      });

      yield put({
        type: 'user/changeNotifyCount',
        payload: {
          totalCount: notices.length,
          unreadCount: notices.filter((item) => !item.read).length,
        },
      });
    },
  },

  reducers: {
    setSiteData(state, { payload }): GlobalModelState {
      return {
        notices: [],
        collapsed: false,
        ...state,
        siteData: payload || [],
      };
    },
    setCurrentSite(state, { payload }): GlobalModelState {
      return {
        siteData: [],
        notices: [],
        collapsed: false,
        ...state,
        selectedSite: state?.siteData.find(temp => temp.value = payload)
      };
    },
    changeLayoutCollapsed(state, { payload }): GlobalModelState {
      return {
        siteData: [],
        notices: [],
        ...state,
        collapsed: payload,
      };
    },
    saveNotices(state, { payload }): GlobalModelState {
      return {
        siteData: [],
        collapsed: false,
        ...state,
        notices: payload,
      };
    },
    saveClearedNotices(state, { payload }): GlobalModelState {
      return {
        siteData: [],
        ...state,
        collapsed: false,
        notices: state?.notices.filter((item): boolean => item.type !== payload) || [],
      };
    },
  },

  subscriptions: {
    setup({ history }): void {
      // Subscribe history(url) change, trigger `load` action if pathname is `/`
      history.listen(({ pathname, search }): void => {
        if (typeof window.ga !== 'undefined') {
          window.ga('send', 'pageview', pathname + search);
        }
      });
    },
  },
};

export default GlobalModel;
