import { GlobalOutlined } from '@ant-design/icons';
import { Menu } from 'antd';
import { connect, Dispatch, SiteSelectItem, GlobalModelState } from 'umi';
import React, { useEffect } from 'react';
import classNames from 'classnames';
import HeaderDropdown from '../HeaderDropdown';
import styles from './index.less';

interface SelectLangProps {
  dispatch: Dispatch;
  siteData: SiteSelectItem[];
  selectedSite?: SiteSelectItem;
  className?: string;
}

const SelectLang: React.FC<SelectLangProps> = (props) => {
  const { className, siteData, dispatch, selectedSite } = props;
  useEffect(() => {
    dispatch({
      type: 'global/fetchSites',
    })
  }, [])

  const langMenu = (
    <Menu
      className={styles.menu}
      selectedKeys={selectedSite ? [selectedSite.num] : []}
      onClick={({ key }) => {
        dispatch({
          type: 'global/setCurrentSite',
          payload: key,
        })
      }}
    >
      {siteData.map((site) => (
        <Menu.Item key={site.num}>
          {site.name}
        </Menu.Item>
      ))}
    </Menu>
  );
  return (
    <HeaderDropdown overlay={langMenu} placement="bottomRight">
      <span className={classNames(styles.dropDown, className)}>
        <GlobalOutlined title="语言" />
      </span>
    </HeaderDropdown>
  );
};

export default connect(({ global: { siteData, selectedSite } }: { global: GlobalModelState }) => {
  return {
    siteData,
    selectedSite,
  }
})(SelectLang);
