import React, { useRef, useState } from 'react';
import { PageHeaderWrapper } from '@ant-design/pro-layout';
import ProTable, { ActionType as TableAction, ProColumns, QuerySymbol } from '@/components/ListTable';
import { AdminListProps, Administrator } from './data';
import { page, Submit, Delete } from './service';
import { Button, message, Tooltip, Dropdown, Menu } from 'antd';
import ModalForm, { ModalFormState, ModalFormAction } from '@/components/ModalForm';
import { EditOutlined, DownOutlined } from '@ant-design/icons';
import { DeleteConfirm } from '@/utils/msg';

const handleDelete = (rows: Administrator[] | undefined, action: React.MutableRefObject<TableAction | undefined>) => {
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

const AdminList: React.FC<AdminListProps> = () => {
    const [editForm, setEditForm] = useState<ModalFormState>({
        visible: false,
        title: '',
        isUpdate: false,
    });

    const tableAction = useRef<TableAction>();
    const editAction = useRef<ModalFormAction>();
    const columns: ProColumns<Administrator>[] = [{
        dataIndex: 'accountName',
        title: '账户名称',
        querySymbol: QuerySymbol.Like,
    }, {
        dataIndex: 'trueName',
        title: '真实姓名',
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
                                isUpdate: true,
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
            toolBarRender={(action, { selectedRows, selectedRowKeys }) => [
                <Button
                    type="primary"
                    onClick={() => {
                        setEditForm({
                            visible: true,
                            title: '添加管理员',
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
                return new Promise(resolve => {
                    if (value.groupNum instanceof Array)
                        value.groupNum = value.groupNum[value.groupNum.length - 1];

                    Submit(value).then(res => {
                        resolve();
                        if (res.isSuccess) {
                            message.success('操作成功');
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