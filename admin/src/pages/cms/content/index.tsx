import React, { useRef, useState, useEffect } from 'react';
import { connect, Loading, GlobalModelState } from 'umi'
import { ContentManagementProps, ColumnContentItem, ContentModelState, ColumnItem, ContentEditState } from "./data";
import { PageHeaderWrapper } from '@ant-design/pro-layout';
import { Card, Row, Col, Menu, Empty, Button, Tooltip } from 'antd';
import ProTable, { ActionType as TableAction, ProColumns } from '@/components/ListTable';
import { page } from './service';
import moment from 'moment';
import { PlusOutlined, EditOutlined } from '@ant-design/icons';
import ContentEditDrawer from './components/ContentEditDrawer';
import { lowerCaseFieldName } from '@/utils/utils';

const { SubMenu } = Menu;

const renderMenu = (data: ColumnItem[]) => {
    return data.map(temp => {
        if (!temp.children || temp.children.length <= 0) return <Menu.Item key={temp.num}>{temp.name}</Menu.Item>
        return <SubMenu key={temp.num} title={temp.name}>
            {renderMenu(temp.children)}
        </SubMenu>
    })
}

const ContentManagement: React.FC<ContentManagementProps> = ({
    dispatch,
    columnData,
    loadingColumnData,
    loadingTableColumns,
    currentColumnNum,
    currentTableFields,
    currentSite
}) => {
    const tableAction = useRef<TableAction>();
    const [contentTableColumns, setContentTableColumns] = useState<ProColumns<ColumnContentItem>[]>([]);
    const [columnOpenKeys, setColumnOpenKeys] = useState<string[]>([]);
    const [rootSubmenuKeys, setRootSubmenuKeys] = useState<string[]>([]);
    const [contentEdit, setContentEdit] = useState<ContentEditState>({
        visible: false,
    })

    useEffect(() => {
        setRootSubmenuKeys(columnData.map(temp => temp.num));
    }, [columnData])

    useEffect(() => {
        if (currentSite) {
            dispatch({
                type: 'content/fetchColumns',
                siteNum: currentSite.num
            })
        }
    }, [currentSite])

    useEffect(() => {
        if (currentTableFields && currentTableFields.length > 0) {
            const columns: ProColumns<ColumnContentItem>[] = currentTableFields.map((temp): ProColumns<ColumnContentItem> => {
                return {
                    dataIndex: lowerCaseFieldName(temp.name),
                    title: temp.explain
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
        }
    }, [currentTableFields])


    return <PageHeaderWrapper>
        <Card bordered={false} bodyStyle={{ padding: 0 }} loading={loadingColumnData}>
            <Row>
                <Col>
                    <Menu
                        style={{ width: 256 }}
                        mode="inline"
                        selectedKeys={currentColumnNum ? [currentColumnNum] : []}
                        openKeys={columnOpenKeys}
                        onOpenChange={openKeys => {
                            if (Array.isArray(openKeys)) {
                                const latestOpenKey = openKeys.find((key) => columnOpenKeys.indexOf(key.toString()) === -1)?.toString();
                                if (rootSubmenuKeys.indexOf(latestOpenKey || '') === -1) {
                                    setColumnOpenKeys(openKeys as string[])
                                } else {
                                    setColumnOpenKeys(latestOpenKey ? [latestOpenKey] : []);
                                }
                            }
                        }}
                        onSelect={({ item, key, keyPath, selectedKeys, domEvent }) => {
                            dispatch({
                                type: 'content/fetchColumnTableColumns',
                                payload: key
                            })
                        }}
                    >
                        {renderMenu(columnData)}
                    </Menu>
                </Col>
                <Col flex={1}>
                    <Card bordered={false} loading={loadingTableColumns}>
                        {currentColumnNum && currentTableFields && currentTableFields.length > 0 ? <ProTable<ColumnContentItem>
                            headerTitle="内容列表"
                            actionRef={tableAction}
                            request={(params, sort, query) => {
                                params['columnNum'] = currentColumnNum;
                                return page({
                                    params,
                                    sort,
                                    query
                                });
                            }}
                            columns={contentTableColumns}
                            rowSelection={{}}
                            pagination={false}
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
                                >新增</Button>
                            ]}
                        /> : <Empty description="未选择栏目或栏目未设置字段" />}
                    </Card>
                </Col>
            </Row>
        </Card>
        <ContentEditDrawer
            {...contentEdit}
            columnNum={currentColumnNum || ''}
            columnFields={currentTableFields || []}
            onClose={(isSuccess) => {
                setContentEdit({
                    ...contentEdit,
                    visible: false
                })

                if (isSuccess) {
                    tableAction.current?.reload();
                }
            }}
        />
    </PageHeaderWrapper>
}

export default connect(({ content: { columns, columnTableFields, currentColumnNum }, loading, global }: { global: GlobalModelState, content: ContentModelState, loading: Loading }) => {

    return {
        columnData: columns,
        currentColumnNum,
        currentSite: global.selectedSite,
        currentTableFields: columnTableFields && currentColumnNum ? columnTableFields[currentColumnNum] : [],
        loadingColumnData: loading.effects['content/fetchColumns'],
        loadingTableColumns: loading.effects['content/fetchColumnTableColumns'],
    }
})(ContentManagement);