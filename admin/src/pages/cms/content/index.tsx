import React, { useState, useEffect, Suspense, Fragment } from 'react';
import { connect, Loading, GlobalModelState } from 'umi'
import { ContentManagementProps, ContentModelState, ColumnItem } from "./data";
import { PageHeaderWrapper, PageLoading } from '@ant-design/pro-layout';
import { Card, Row, Col, Menu, Empty, PageHeader, Button } from 'antd';
import ContentManage from './components/ContentManage';
import SeoForm from '@/components/Content/SeoForm';

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
    currentColumn,
    currentTableFields,
    currentSite
}) => {
    const [columnOpenKeys, setColumnOpenKeys] = useState<string[]>([]);
    const [rootSubmenuKeys, setRootSubmenuKeys] = useState<string[]>([]);
    const [seoForm, setSeoForm] = useState<{
        visible: boolean;
        getDataUrl: string;
        setDataUrl: string;
        param?: { [key: string]: any };
    }>({
        visible: false,
        getDataUrl: '',
        setDataUrl: '',
    });


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

    return <PageHeaderWrapper
        extra={<Fragment>
            <Button.Group>
                <Button
                    type="primary"
                    onClick={() => {
                        setSeoForm({
                            visible: true,
                            getDataUrl: '/Api/CMS/Site/GetSeo',
                            setDataUrl: '/Api/CMS/Site/UpdateSeo',
                            param: {
                                num: currentSite?.num,
                            }
                        })
                    }}
                >站点SEO设置</Button>
            </Button.Group>
        </Fragment>}
    >
        <Card bordered={false} bodyStyle={{ padding: 0 }} loading={loadingColumnData}>
            <Row>
                <Col flex="260px">
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
                        onSelect={({ key }) => {
                            dispatch({
                                type: 'content/fetchColumnTableColumns',
                                payload: key
                            })
                        }}
                    >
                        {renderMenu(columnData)}
                    </Menu>
                </Col>
                <Col flex="1 1 1000px">
                    <Card
                        bordered={false}
                        loading={loadingTableColumns}
                        bodyStyle={{ padding: 10, paddingTop: 15 }}
                    >
                        {currentColumnNum && currentTableFields && currentTableFields.length > 0 ?
                            <Suspense fallback={<PageLoading />}>
                                <ContentManage
                                    currentColumn={currentColumn}
                                    currentColumnNum={currentColumnNum}
                                    currentTableFields={currentTableFields}
                                />
                            </Suspense> : <Empty description="未选择栏目或栏目未设置字段" />}
                    </Card>
                </Col>
            </Row>
        </Card>
        <Suspense fallback={<PageLoading />}>
            <SeoForm
                {...seoForm}
                onClose={() => {
                    setSeoForm({
                        ...seoForm,
                        visible: false,
                    })
                }}
            />
        </Suspense>
    </PageHeaderWrapper>
}

export default connect(({ content: { columns, columnTableFields, currentColumnNum, currentColumn }, loading, global }: { global: GlobalModelState, content: ContentModelState, loading: Loading }) => {

    return {
        columnData: columns,
        currentColumnNum,
        currentColumn,
        currentSite: global.selectedSite,
        currentTableFields: columnTableFields && currentColumnNum ? columnTableFields[currentColumnNum] : [],
        loadingColumnData: loading.effects['content/fetchColumns'],
        loadingTableColumns: loading.effects['content/fetchColumnTableColumns'],
    }
})(ContentManagement);