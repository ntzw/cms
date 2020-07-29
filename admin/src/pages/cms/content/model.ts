import { Effect, Reducer } from 'umi';
import { PageResponse, HandleResult } from '@/utils/request';
import { page } from '../columnlist/service'
import { ContentModelState, ColumnItem } from './data';
import { ColumnField } from '@/components/Content/data';
import { GetColumnContentFields } from '@/components/Content/service';

export interface ContentModelType {
    namespace: 'content';
    state: ContentModelState;
    effects: {
        fetchColumns: Effect;
        fetchColumnTableColumns: Effect;
    };
    reducers: {
        saveColumns: Reducer<ContentModelState>;
        saveCurrentColumnNum: Reducer<ContentModelState>;
        saveColumnTableFields: Reducer<ContentModelState>;
    };
}

const ContentModel: ContentModelType = {
    namespace: 'content',
    state: {
        columns: [],
    },

    effects: {
        *fetchColumns({ siteNum }, { call, put }) {
            const response: PageResponse<ColumnItem> = yield call(page, {
                siteNum,
            });
            if (response.isSuccess) {
                yield put({
                    type: 'saveColumns',
                    payload: response.data || []
                })
            }
        },
        *fetchColumnTableColumns({ payload }, { call, select, put }) {
            yield put({
                type: 'saveCurrentColumnNum',
                payload: payload,
            })

            const oldTableColumns = yield select(({ content }: { content: ContentModelState }) => content.columnTableFields);
            if (!oldTableColumns || !oldTableColumns[payload]) {
                const res: HandleResult<{ fields: ColumnField[]; }> = yield call(GetColumnContentFields, payload);
                if (res.isSuccess && res.data?.fields && res.data.fields.length > 0) {
                    yield put({
                        type: 'saveColumnTableFields',
                        columnNum: payload,
                        fields: res.data?.fields || []
                    })
                }
            }
        }
    },

    reducers: {
        saveColumns(state, { payload }) {
            return {
                ...state,
                columns: payload || [],
            };
        },
        saveCurrentColumnNum(state, { payload }) {
            return {
                columns: [],
                ...state,
                currentColumnNum: payload,
                currentColumn: getColumnByNum(state?.columns || [], payload)
            };
        },
        saveColumnTableFields(state, { columnNum, fields }) {
            return {
                columns: [],
                ...state,
                columnTableFields: {
                    ...state?.columnTableFields,
                    [columnNum]: fields || []
                }
            }
        }
    },
};

function getColumnByNum(columns: ColumnItem[], num: string): ColumnItem | undefined {
    for (let i = 0; i < columns.length; i++) {
        const element = columns[i];
        if (element.num === num) return element;
        if (element.children instanceof Array && element.children.length > 0) return getColumnByNum(element.children, num);
    }

    return undefined;
}

export default ContentModel;
