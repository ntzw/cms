import React, { useRef, useState } from 'react';
import { PageHeaderWrapper } from '@ant-design/pro-layout';
import ProTable, { ActionType as TableAction, ProColumns } from '@/components/ListTable';
import { ColumnListProps, Column, ColumnFieldListPropsState } from './data';
import { page, Submit } from './service';
import { Button, message, Tooltip } from 'antd';
import ModalForm, { ModalFormState, ModalFormAction } from '@/components/ModalForm';
import { EditOutlined, DeleteOutlined, ControlOutlined } from '@ant-design/icons';
import moment from 'moment';
import { connect, GlobalModelState } from 'umi';
import ColumnFieldList from './components/columnfieldlist';

const ColumnList: React.FC<ColumnListProps> = ({ currentSite }) => {
    const [editForm, setEditForm] = useState<ModalFormState>({
        visible: false,
        title: '',
        isUpdate: false,
    });

    const [editColumnField, setEditColumnField] = useState<ColumnFieldListPropsState>({
        visible: false,
    })

    const tableAction = useRef<TableAction>();
    const editAction = useRef<ModalFormAction>();
    const columns: ProColumns<Column>[] = [{
        dataIndex: 'name',
        title: '名称',
        valueType: 'option',
    }, {
        dataIndex: 'modelName',
        title: '模型名称',
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
                                title: '编辑栏目',
                                isUpdate: true,
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

                        }}
                    />
                </Tooltip>
                <Tooltip title="字段">
                    <Button
                        icon={<ControlOutlined />}
                        type="primary"
                        onClick={() => {
                            setEditColumnField({
                                visible: true,
                                column: record,
                            })
                        }}
                    />
                </Tooltip>
            </Button.Group>
        }
    }];


    return <PageHeaderWrapper>
        <ProTable<Column>
            headerTitle="栏目列表"
            actionRef={tableAction}
            request={(params) => page(params)}
            columns={columns}
            rowSelection={{}}
            pagination={false}
            params={{
                siteNum: currentSite?.num,
            }}
            toolBarRender={(action) => [
                <Button
                    type="primary"
                    onClick={() => {
                        setEditForm({
                            visible: true,
                            isUpdate: false,
                            title: '添加栏目'
                        })
                    }}
                >
                    添加
                </Button>,
            ]}
        />
        <ModalForm<Column>
            {...editForm}
            actionRef={editAction}
            fields="/api/CMS/Column/FormFields"
            onClose={() => {
                setEditForm({
                    ...editForm,
                    visible: false,
                })
            }}
            onFinish={(value) => {
                return new Promise(resolve => {
                    value.siteNum = currentSite?.num || '';
                    if (value.parentNum instanceof Array)
                        value.parentNum = value.parentNum[value.parentNum.length - 1];

                    Submit(value).then(res => {
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
                if (field.name.toLocaleLowerCase() === 'parentnum') {
                    return {
                        siteNum: currentSite?.num,
                    }
                }

                return null;
            }}
        />
        <ColumnFieldList
            {...editColumnField}
            onClose={() => {
                setEditColumnField({
                    ...editColumnField,
                    visible: false,
                })
            }}
        />
    </PageHeaderWrapper >
}

export default connect(({ global: { selectedSite } }: { global: GlobalModelState }) => {
    return {
        currentSite: selectedSite
    }
})(ColumnList);