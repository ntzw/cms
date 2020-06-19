import { Reducer, Effect } from 'umi';
import { postAjax } from '@/services/global';

export interface ModelState {
    tableData: TableType[]
}

export interface TableType {
    name: string;
    fields?: TableField[]
}

export interface TableField {
    columnName: string;
    dataLength: number;
    dataType: string;
    description: string;
    isNullable: boolean;
}

export interface ModelType {
    namespace: 'codeGenerate';
    state: ModelState;
    effects: {
        load: Effect;
    };
    reducers: {
        saveTableData: Reducer<ModelState>;
    };
}

export interface HandleResult<T = any> {
    isSuccess: boolean;
    message: string;
    data?: T;
}

const GlobalModel: ModelType = {
    namespace: 'codeGenerate',
    state: {
        tableData: [],
    },

    effects: {
        *load(_, { call, put }) {
            const res: HandleResult<string[]> = yield call(postAjax, '/api/Utils/CodeGenerate/GetAllTable');
            if (res.isSuccess) {
                const tableData: TableType[] = [];
                function* loadFields(tableName: string) {
                    const fieldsRes: HandleResult<TableField[]> = yield call(postAjax, '/api/Utils/CodeGenerate/GetTableFields', {
                        name: tableName,
                    });
                    if (fieldsRes.isSuccess) {
                        tableData.push({
                            name: tableName,
                            fields: fieldsRes.data || []
                        })
                    }
                }

                const data = (res.data || []);
                for (let i = 0; i < data.length; i++) {
                    yield loadFields(data[i]);
                }

                yield put({
                    type: 'saveTableData',
                    payload: tableData
                })
            }
        },
    },

    reducers: {
        saveTableData(state = { tableData: [] }, { payload }): ModelState {
            return {
                ...state,
                tableData: payload,
            };
        },
    },
};

export default GlobalModel;
