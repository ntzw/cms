import React, { useRef, useState } from 'react';
import { PageHeaderWrapper } from '@ant-design/pro-layout';
import ProTable, { ActionType as TableAction, ProColumns, QuerySymbol } from '@/components/ListTable';
import { RoleListProps, Role } from './data';
import { page, Submit, Delete } from './service';
import { Button, message, Tooltip, Modal } from 'antd';
import ModalForm, { ModalFormState, ModalFormAction } from '@/components/ModalForm';
import { EditOutlined, DeleteOutlined } from '@ant-design/icons';
import moment from 'moment';
import { DeleteConfirm } from '@/utils/msg';

const RoleList: React.FC<RoleListProps> = () => {
    const [editForm, setEditForm] = useState<ModalFormState>({
        visible: false,
        title: ''
    });

    const tableAction = useRef<TableAction>();
    const editAction = useRef<ModalFormAction>();
    const columns: ProColumns<Role>[] = [{
        dataIndex: 'name',
        title: '名称',
        valueType: 'option',
    }, {
        dataIndex: 'desc',
        title: '描述',
        valueType: 'option',
    }, {
        dataIndex: 'createDate',
        title: '创建时间',
        valueType: 'option',
        sorter: true,
        render: (text) => {
            return typeof text === 'string' && text && moment(text).format('YYYY-MM-DD HH:mm:ss');
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
                                title: '编辑角色',
                                params: {
                                    id: record.id,
                                }
                            })
                        }}
                    />
                </Tooltip>
                <Tooltip title="删除">
                    <Button
                        icon={<DeleteOutlined />}
                        type="primary"
                        danger
                        onClick={() => {
                            const ids = (function getDeleteNum(item: Role): number[] {
                                let data: number[] = [];
                                data.push(item.id);

                                item.children?.forEach(temp => {
                                    data = data.concat(getDeleteNum(temp));
                                });

                                return data;
                            })(record);

                            if (ids.length <= 0) {
                                message.error('请选择要删除的数据');
                                return;
                            }

                            DeleteConfirm(() => {
                                Delete(ids).then(res => {
                                    if (res.isSuccess) {
                                        message.success('删除成功');
                                        tableAction.current?.reload();
                                    } else {
                                        message.error(res.message || '删除失败');
                                    }
                                })
                            })
                        }}
                    />
                </Tooltip>
            </Button.Group>
        }
    }];


    return <PageHeaderWrapper>
        <ProTable<Role>
            headerTitle="角色列表"
            actionRef={tableAction}
            request={() => page()}
            columns={columns}
            rowSelection={{}}
            pagination={false}
            toolBarRender={(action, { selectedRowKeys }) => [
                <Button
                    type="primary"
                    onClick={() => {
                        setEditForm({
                            visible: true,
                            title: '添加角色'
                        })
                    }}
                >
                    添加
                </Button>,
            ]}
        >

        </ProTable>
        <ModalForm<Role>
            {...editForm}
            actionRef={editAction}
            fields="/api/Account/Role/FormFields"
            onClose={() => {
                setEditForm({
                    ...editForm,
                    visible: false,
                })
            }}
            onFinish={(value) => {
                editAction.current?.setSubmitLoading(true);

                if (value.parentNum instanceof Array)
                    value.parentNum = value.parentNum[value.parentNum.length - 1];

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

export default RoleList;