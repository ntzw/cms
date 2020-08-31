import React from 'react'
import { connect, GlobalModelState, SiteSelectItem } from 'umi'
import { PageHeaderWrapper } from '@ant-design/pro-layout';
import { Card, Row, Col } from 'antd';

interface TemplateManagementProps {
    currentSite?: SiteSelectItem;
}

const TemplateManagement: React.FC<TemplateManagementProps> = () => {
    return <PageHeaderWrapper>
        <Card bordered={false} bodyStyle={{ padding: 0 }}>
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
    </PageHeaderWrapper>
}

export default connect(({ global }: { global: GlobalModelState }) => {
    return {
        currentSite: global.selectedSite,
    }
})(TemplateManagement);

