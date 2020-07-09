import React, { useRef, useState } from 'react';
import ProTable, { ActionType as TableAction, ProColumns, QuerySymbol } from '@/components/ListTable';
import { ModelFieldListProps, ModelField } from '../data';
import { fieldPage } from '../service';
import { ModalFormState } from '@/components/ModalForm';
import { Drawer } from 'antd';


const ModelFieldList: React.FC<ModelFieldListProps> = ({ visible, onClose, modelItem }) => {
    const [] = useState<ModalFormState>({
        visible: false,
        title: ''
    });

    const tableAction = useRef<TableAction>();
    const columns: ProColumns<ModelField>[] = [{
        dataIndex: 'name',
        title: '字段名称',
        querySymbol: QuerySymbol.Like,
    }, {
        dataIndex: 'explain',
        title: '说明',
        querySymbol: QuerySymbol.Like,
    }, {
        dataIndex: 'optionType',
        title: '操作类型',
        valueType: 'option',
    }];


    return <Drawer
        placement="left"
        width="90%"
        visible={visible}
        onClose={onClose}
        title={`模型 ${modelItem?.explain} 字段信息`}
    >
        <ProTable<ModelField>
            headerTitle="模型字段列表"
            actionRef={tableAction}
            params={{
                modelNum: modelItem?.num,
            }}
            request={(params, sort, querySymbol) => fieldPage(params, sort, querySymbol)}
            columns={columns}
            rowSelection={{}}
        >

        </ProTable>
    </Drawer>
}

export default ModelFieldList;