import React, { useRef, useState, useEffect } from 'react';
import { ContentManageProps, ColumnContentItem, ColumnItem, ContentEditState, CategoryManagementState } from '../data';
import ProTable, { ActionType as TableAction, ProColumns } from '@/components/ListTable';
import { ContentPage, GetEditValue, ContentSubmit } from '../service';
import { ColumnField, ContentFormAction } from '@/components/Content/data';
import { lowerCaseFieldName } from '@/utils/utils';
import moment from 'moment';
import { Button, Tooltip, Spin, message } from 'antd';
import { EditOutlined, PlusOutlined, BarsOutlined } from '@ant-design/icons';
import ContentEditDrawer from './ContentEditDrawer';
import CategoryManagement from './CategoryManagement';
import ContentForm from '@/components/Content/ContentForm';
import { Column } from '../../columnlist/data';


const ContentListTable: React.FC<{
    currentColumn?: ColumnItem;
    currentColumnNum?: string;
    currentTableFields?: ColumnField[];
}> = ({
    currentColumn,
    currentColumnNum,
    currentTableFields
}) => {
        const tableAction = useRef<TableAction>();
        const [contentTableColumns, setContentTableColumns] = useState<ProColumns<ColumnContentItem>[]>([]);
        const [contentEdit, setContentEdit] = useState<ContentEditState>({
            visible: false,
        });
        const [categoryManage, setCategoryManage] = useState<CategoryManagementState>({
            visible: false,
        });
        const [loadColumns, setLoadColumns] = useState(false);
        const contentAction = useRef<ContentFormAction>();

        useEffect(() => {
            if (currentTableFields && currentTableFields.length > 0) {
                setLoadColumns(true);
                const columns: ProColumns<ColumnContentItem>[] = currentTableFields.map((temp): ProColumns<ColumnContentItem> => {
                    return {
                        dataIndex: lowerCaseFieldName(temp.name),
                        title: temp.explain,
                        ellipsis: true,
                        width: 100,
                    }
                });

                columns.push({
                    dataIndex: 'createDate',
                    title: '创建时间',
                    render: (value) => {
                        return moment(value + '').format('YYYY-MM-DD HH:mm')
                    }
                })

                columns.push({
                    dataIndex: '_',
                    title: '操作',
                    render: (_, row) => {
                        return <Button.Group>
                            <Tooltip title="编辑">
                                <Button
                                    icon={<EditOutlined />}
                                    type="primary"
                                    onClick={() => {
                                        setContentEdit({
                                            visible: true,
                                            itemNum: row.num,
                                        })
                                    }}
                                />
                            </Tooltip>
                        </Button.Group>
                    }
                })

                setContentTableColumns(columns);
                tableAction.current?.reload();
                setLoadColumns(false);
            }
        }, [currentTableFields])


        return <Spin spinning={loadColumns}>
            <ProTable<ColumnContentItem>
                headerTitle={`${currentColumn?.name} 内容列表`}
                actionRef={tableAction}
                request={(params, sort, query) => {
                    params['columnNum'] = currentColumnNum;
                    return ContentPage({
                        params,
                        sort,
                        query
                    });
                }}
                columns={contentTableColumns}
                rowSelection={{}}
                pagination={{
                    size: "default"
                }}
                params={{

                }}
                toolBarRender={(action) => [
                    <Button
                        icon={<PlusOutlined />}
                        type="primary"
                        onClick={() => {
                            setContentEdit({
                                visible: true,
                            })
                        }}
                    >新增</Button>,
                    currentColumn?.isCategory && <Button
                        icon={<BarsOutlined />}
                        onClick={() => {
                            setCategoryManage({
                                visible: true,
                            })
                        }}
                    >类别管理</Button>
                ]}
            />
            <ContentEditDrawer
                {...contentEdit}
                actionRef={contentAction}
                afterVisibleChange={(visible) => {
                    if (!visible) {
                        tableAction.current?.reload();
                    }
                }}
                onClose={(isSuccess) => {
                    setContentEdit({
                        ...contentEdit,
                        visible: false
                    })
                }}
            />
            <CategoryManagement
                {...categoryManage}
                onClose={() => {
                    contentAction.current?.reoladFieldItem();
                    setCategoryManage({
                        ...categoryManage,
                        visible: false,
                    })
                }}
            />
        </Spin>
    }

const ContentSinageEdit: React.FC<{
    currentTableFields?: ColumnField[];
    currentColumnNum?: string;
    currentColumn?: Column;
}> = ({
    currentTableFields,
    currentColumnNum,
    currentColumn
}) => {
        const formAction = useRef<ContentFormAction>();
        const [submiting, setSubmiting] = useState(false);
        const [loadValue, setLoadValue] = useState(false);
        const [editValue, setEditValue] = useState<{ [key: string]: any }>();

        useEffect(() => {
            setLoadValue(true);
            GetEditValue('', currentColumnNum || '').then(res => {
                setEditValue(res.data || {});
                formAction.current?.setValue(res.data);

                setTimeout(() => {
                    setLoadValue(false);
                }, 500);
            })
        }, [currentColumnNum])

        return <div>
            <Spin spinning={loadValue} >
                <ContentForm
                    actionRef={formAction}
                    columnFields={currentTableFields || []}
                    columnNum={currentColumnNum || ''}
                    isSeo={currentColumn?.isSeo}
                    isCategory={currentColumn?.isCategory}
                    onFinish={(value) => {
                        setSubmiting(true);
                        return new Promise(resolve => {
                            const newValue = {
                                ...editValue,
                                ...value,
                                columnNum: currentColumnNum,
                            }

                            ContentSubmit(newValue).then(res => {
                                if (res.isSuccess) {
                                    message.success('数据提交成功');
                                } else {
                                    message.error(res.message || '数据提交失败');
                                }

                                resolve();
                                formAction.current?.reoladFieldItem();
                                setSubmiting(false);
                            })
                        })
                    }}
                />
                <div style={{ textAlign: 'center' }}>
                    <Button
                        type="primary"
                        loading={submiting}
                        onClick={() => {
                            formAction.current?.submit();
                        }}
                    >提交</Button>
                    <Button
                        danger
                        style={{ marginLeft: 5 }}
                        loading={submiting}
                        onClick={() => {
                            formAction.current?.clear();
                        }}
                    >重置</Button>
                </div>
            </Spin>
        </div>
    }


const ContentManage: React.FC<ContentManageProps> = ({
    currentColumn,
    currentColumnNum,
    currentTableFields
}) => {
    return currentColumn?.isSingle ?
        <ContentSinageEdit
            currentColumnNum={currentColumnNum}
            currentTableFields={currentTableFields}
            currentColumn={currentColumn}
        /> :
        <ContentListTable
            currentColumn={currentColumn}
            currentColumnNum={currentColumnNum}
            currentTableFields={currentTableFields}
        />
}

export default ContentManage;