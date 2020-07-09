import React, { useRef, useState } from 'react';
import { PageHeaderWrapper } from '@ant-design/pro-layout';
import ProTable, { ActionType as TableAction, ProColumns, QuerySymbol } from '@/components/ListTable';
import { ColumnListProps, Column } from './data';
import { page, Submit, Delete } from './service';
import { Button, message, Tooltip, Badge, Dropdown, Menu } from 'antd';
import ModalForm, { ModalFormState, ModalFormAction } from '@/components/ModalForm';
import { EditOutlined, DownOutlined } from '@ant-design/icons';
import { DeleteConfirm } from '@/utils/msg';

const handleDelete = (rows: Column[] | undefined, action: React.MutableRefObject<TableAction | undefined>) => {
    if (!rows || rows.length <= 0) {
        message.error('请选择要删除的数据');
        return;
    }

    DeleteConfirm(() => {
        const ids = rows.map(temp => temp.id);
        Delete(ids).then(res => {
            if (res.isSuccess) {
                message.success('删除成功');
                action.current?.reload();
                action.current?.clearSelected();
            } else {
                message.error(res.message || '删除失败');
            }
        })
    })
}

const AdminList: React.FC<ColumnListProps> = () => {
    const [editForm, setEditForm] = useState<ModalFormState>({
        visible: false,
        title: ''
    });

    const tableAction = useRef<TableAction>();
    const editAction = useRef<ModalFormAction>();
    const columns: ProColumns<Column>[] = [{
        dataIndex: 'name',
        title: '站点名称',
        querySymbol: QuerySymbol.Like,
    }, {
        dataIndex: 'host',
        title: '站点域名',
        querySymbol: QuerySymbol.Like,
    }, {
        dataIndex: 'isDefault',
        title: '是否默认站点',
        valueType: 'option',
        render: (value) => {
            return value ? <Badge status="success" text="是" /> : <Badge status="default" text="否" />;
        }
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
                                title: '编辑站点信息',
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
        <ProTable<Column>
            headerTitle="站点列表"
            actionRef={tableAction}
            request={(params, sort, querySymbol) => page(params, sort, querySymbol)}
            columns={columns}
            rowSelection={{}}
            toolBarRender={(action, { selectedRows, selectedRowKeys }) => {
                return [
                    <Button
                        type="primary"
                        onClick={() => {
                            setEditForm({
                                visible: true,
                                title: '添加站点'
                            })
                        }}
                    >
                        添加
                    </Button>,
                    selectedRows && selectedRows.length > 0 && (
                        <Dropdown
                            overlay={
                                <Menu
                                    onClick={async (e) => {
                                        if (e.key === 'remove') {
                                            handleDelete(selectedRows, tableAction);
                                        }
                                    }}
                                    selectedKeys={[]}
                                >
                                    <Menu.Item key="remove">批量删除</Menu.Item>
                                </Menu>
                            }
                        >
                            <Button>
                                批量操作 <DownOutlined />
                            </Button>
                        </Dropdown>
                    ),
                ]
            }}
        >

        </ProTable>
        <ModalForm<Column>
            {...editForm}
            actionRef={editAction}
            fields="/Api/CMS/Column/FormFields"
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
                        editAction.current?.clear();
                        editAction.current?.close();
                        tableAction.current?.reload();
                    } else {
                        message.error(res.message || '操作失败');
                    }
                });
            }}
        />
    </PageHeaderWrapper >
}

export default AdminList;