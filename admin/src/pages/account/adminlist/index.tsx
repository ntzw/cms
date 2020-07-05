import React, { useRef, useState } from 'react';
import { PageHeaderWrapper } from '@ant-design/pro-layout';
import ProTable, { ActionType as TableAction, ProColumns, QuerySymbol } from '@/components/ListTable';
import { AdminListProps, Administrator } from './data';
import { page, Submit } from './service';
import { Button, message, Tooltip } from 'antd';
import ModalForm, { ModalFormState, ModalFormAction } from '@/components/ModalForm';
import { EditOutlined } from '@ant-design/icons';

const AdminList: React.FC<AdminListProps> = () => {
    const [editForm, setEditForm] = useState<ModalFormState>({
        visible: false,
        title: ''
    });

    const tableAction = useRef<TableAction>();
    const editAction = useRef<ModalFormAction>();
    const columns: ProColumns<Administrator>[] = [{
        dataIndex: 'accountName',
        title: '账户名称',
        sorter: true,
        querySymbol: QuerySymbol.Like,
    }, {
        dataIndex: 'trueName',
        title: '真实姓名',
        sorter: true,
    }, {
        dataIndex: '-',
        title: '操作',
        valueType: 'option',
        render: (_, record) => {
            return <Button.Group>
                <Tooltip title="编辑">
                    <Button
                        icon={<EditOutlined />}
                        onClick={() => {
                            setEditForm({
                                visible: true,
                                title: '编辑管理员',
                                params: {
                                    id: record.id,
                                }
                            })
                        }}
                    />
                </Tooltip>
            </Button.Group>
        }
    }];


    return <PageHeaderWrapper>
        <ProTable<Administrator>
            headerTitle="管理员列表"
            actionRef={tableAction}
            request={(params, sort, querySymbol) => page(params, sort, querySymbol)}
            columns={columns}
            rowSelection={{}}
            toolBarRender={(action, { selectedRowKeys }) => [
                <Button
                    type="primary"
                    onClick={() => {
                        setEditForm({
                            visible: true,
                            title: '添加管理员'
                        })
                    }}
                >
                    添加
                </Button>,
            ]}
        >

        </ProTable>
        <ModalForm<Administrator>
            {...editForm}
            actionRef={editAction}
            fields="/api/Account/Admin/FormFields"
            onClose={() => {
                setEditForm({
                    ...editForm,
                    visible: false,
                })
            }}
            onFinish={(value) => {
                editAction.current?.setSubmitLoading(true);
                Submit(value).then(res => {
                    editAction.current?.setSubmitLoading(false);
                    if (res.isSuccess) {
                        message.success('操作成功');
                        tableAction.current?.reload();
                        editAction.current?.close();
                    } else {
                        message.error(res.message || '操作失败');
                    }
                });
            }}
        />
    </PageHeaderWrapper >
}

export default AdminList;