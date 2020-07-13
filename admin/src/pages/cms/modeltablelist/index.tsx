import React, { useRef, useState } from 'react';
import { PageHeaderWrapper } from '@ant-design/pro-layout';
import ProTable, { ActionType as TableAction, ProColumns, QuerySymbol } from '@/components/ListTable';
import { ModelTableListProps, ModelTable, ModelFieldListPropsState } from './data';
import { page, Submit, Delete } from './service';
import { Button, message, Tooltip, Dropdown, Menu } from 'antd';
import ModalForm, { ModalFormState, ModalFormAction } from '@/components/ModalForm';
import { EditOutlined, DownOutlined, BarsOutlined } from '@ant-design/icons';
import { DeleteConfirm } from '@/utils/msg';
import ModelFieldList from './components/ModelFieldList';

const handleDelete = (rows: ModelTable[] | undefined, action: React.MutableRefObject<TableAction | undefined>) => {
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

const AdminList: React.FC<ModelTableListProps> = () => {
    const [editForm, setEditForm] = useState<ModalFormState>({
        visible: false,
        title: '',
        isUpdate: false,
    });

    const [modelFieldList, setModelFieldList] = useState<ModelFieldListPropsState>({
        visible: false,
    });

    const tableAction = useRef<TableAction>();
    const editAction = useRef<ModalFormAction>();
    const columns: ProColumns<ModelTable>[] = [{
        dataIndex: 'tableName',
        title: '表名',
        querySymbol: QuerySymbol.Like,
    }, {
        dataIndex: 'explain',
        title: '说明',
        querySymbol: QuerySymbol.Like,
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
                                title: '编辑模型信息',
                                isUpdate: true,
                                params: {
                                    id: record.id,
                                }
                            })
                        }}
                    />
                </Tooltip>
                <Tooltip title="字段信息">
                    <Button
                        icon={<BarsOutlined />}
                        type="primary"
                        onClick={() => {
                            setModelFieldList({
                                visible: true,
                                modelItem: record,
                            })
                        }}
                    />
                </Tooltip>
            </Button.Group>
        }
    }];


    return <PageHeaderWrapper>
        <ProTable<ModelTable>
            headerTitle="模型列表"
            actionRef={tableAction}
            request={(params, sort, query) => page({
                params,
                sort,
                query,
            })}
            columns={columns}
            rowSelection={{}}
            toolBarRender={(action, { selectedRows, selectedRowKeys }) => {
                return [
                    <Button
                        type="primary"
                        onClick={() => {
                            setEditForm({
                                visible: true,
                                title: '添加模型',
                                isUpdate: false,
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
        <ModelFieldList
            {...modelFieldList}
            onClose={() => {
                setModelFieldList({
                    ...modelFieldList,
                    visible: false,
                })
            }}
        />
        <ModalForm<ModelTable>
            {...editForm}
            actionRef={editAction}
            fields="/Api/CMS/Model/FormFields"
            onClose={() => {
                setEditForm({
                    ...editForm,
                    visible: false,
                })
            }}
            onFinish={(value) => {
                return new Promise((resolve) => {
                    Submit(value).then(res => {
                        resolve();
                        if (res.isSuccess) {
                            message.success('操作成功');
                            editAction.current?.clear();
                            tableAction.current?.reload();
                            setEditForm({
                                ...editForm,
                                visible: false,
                            })
                        } else {
                            message.error(res.message || '操作失败');
                        }
                    });
                })
            }}
        />
    </PageHeaderWrapper >
}

export default AdminList;