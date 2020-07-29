import React, { useRef, useState } from 'react'
import { CategoryManagementProps, ContentCategory, ContentModelState } from '../data'
import { Drawer, Button, Tooltip, message } from 'antd';
import ProTable, { ActionType as TableAction, ProColumns } from '@/components/ListTable';
import { categoryPage, SubmitCategory, DeleteCategory } from '../service';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import moment from 'moment';
import ModalForm, { ModalFormState, ModalFormAction } from '@/components/ModalForm';
import { Loading, connect, GlobalModelState } from 'umi';
import { DeleteConfirm } from '@/utils/msg';

const CategoryManagement: React.FC<CategoryManagementProps> = ({
    onClose,
    visible,
    currentColumnNum,
    currentColumn,
    currentSite,
}) => {
    const editAction = useRef<ModalFormAction>();
    const [editForm, setEditForm] = useState<ModalFormState>({
        visible: false,
        title: '',
        isUpdate: false,
    })

    const tableAction = useRef<TableAction>();
    const columns: ProColumns<ContentCategory>[] = [{
        dataIndex: 'name',
        title: '名称',
        valueType: 'option',
    }, {
        dataIndex: 'createDate',
        title: '创建时间',
        valueType: 'option',
        render: (text) => {
            return typeof text === 'string' && text && moment(text).format('YYYY-MM-DD HH:mm:ss');
        }
    }, {
        dataIndex: '-',
        title: '操作',
        valueType: 'option',
        render: (_, record) => {
            return <Button.Group>
                <Tooltip title="增加子类别">
                    <Button
                        icon={<PlusOutlined />}
                        type="primary"
                        onClick={() => {
                            setEditForm({
                                visible: true,
                                title: '添加类别',
                                isUpdate: false,
                                params: {
                                    columnNum: currentColumn?.num,
                                    parentNum: record.num,
                                }
                            })
                        }}
                    />
                </Tooltip>
                <Tooltip title="编辑">
                    <Button
                        icon={<EditOutlined />}
                        onClick={() => {
                            setEditForm({
                                visible: true,
                                title: '添加类别',
                                isUpdate: false,
                                params: {
                                    columnNum: currentColumn?.num,
                                    id: record.id
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
                            if (!currentColumnNum) {
                                message.error('未选择栏目');
                                return;
                            }

                            DeleteConfirm(() => {
                                DeleteCategory([record.id], currentColumnNum).then(res => {
                                    if (res.isSuccess) {
                                        message.success('删除成功');
                                        tableAction.current?.reload();
                                    } else {
                                        message.error('删除失败');
                                    }
                                });
                            })
                        }}
                    />
                </Tooltip>
            </Button.Group>
        }
    }];

    return <Drawer
        title={`${currentColumn?.name} 类别管理`}
        placement="right"
        width="80%"
        onClose={onClose}
        visible={visible}
    >
        <ProTable<ContentCategory>
            headerTitle="内容类别"
            actionRef={tableAction}
            request={() => categoryPage(currentColumnNum || '')}
            columns={columns}
            rowSelection={{}}
            pagination={false}
            params={{
                columnNum: currentColumnNum,
            }}
            options={{
                fullScreen: false,
            }}
            toolBarRender={() => {
                return [
                    <Button.Group>
                        <Button
                            icon={<PlusOutlined />}
                            type="primary"
                            onClick={() => {
                                editAction.current?.clear();
                                editAction.current?.setOldValue();

                                setEditForm({
                                    visible: true,
                                    title: '添加类别',
                                    isUpdate: false,
                                    params: {
                                        columnNum: currentColumn?.num,
                                    }
                                })
                            }}
                        >
                            添加
                        </Button>
                    </Button.Group>,
                ]
            }}
        />
        <ModalForm<ContentCategory>
            {...editForm}
            actionRef={editAction}
            fields="/api/CMS/Category/FormFields"
            onClose={() => {
                setEditForm({
                    ...editForm,
                    visible: false,
                })
            }}
            onFinish={(value) => {
                return new Promise(resolve => {
                    value.siteNum = currentSite?.num || '';
                    value.columnNum = currentColumn?.num || '';

                    if (value.parentNum instanceof Array)
                        value.parentNum = value.parentNum[value.parentNum.length - 1];

                    SubmitCategory(value).then(res => {
                        resolve();
                        editAction.current?.reloadFieldItem();
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
            fieldActionParams={(field) => {
                switch (field.name.toLocaleLowerCase()) {
                    case 'parentnum':
                        return {
                            columnNum: currentColumn?.num,
                        }
                    default:
                        return null;
                }
            }}
        />
    </Drawer>
}

export default connect(({ content: { currentColumnNum, currentColumn }, global: { selectedSite }, loading }: { global: GlobalModelState, content: ContentModelState, loading: Loading }) => {
    return {
        currentColumnNum,
        currentColumn,
        currentSite: selectedSite
    }
})(CategoryManagement);