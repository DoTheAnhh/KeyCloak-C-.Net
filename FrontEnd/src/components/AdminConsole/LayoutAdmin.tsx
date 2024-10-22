import { DashboardOutlined, ProductFilled, UserOutlined } from '@ant-design/icons';
import { Flex, Layout, Menu, MenuProps } from 'antd'
import { Content, Footer, Header } from 'antd/es/layout/layout'
import Sider from 'antd/es/layout/Sider';
import React from 'react'
import { Route, Routes, useNavigate } from 'react-router-dom';
import ListRealm from './Realm/ListRealm';
import ListUser from './User/ListUser';

const LayoutAdmin: React.FC = () => {

    const navigator = useNavigate()

    const headerStyle: React.CSSProperties = {
        textAlign: 'center',
        color: '#fff',
        height: 64,
        paddingInline: 48,
        lineHeight: '64px',
        //backgroundColor: '#F5F5F5'
    };

    const siderStyle: React.CSSProperties = {
        textAlign: 'center',
        lineHeight: '120px',
        color: '#fff',
        backgroundColor: '#F5F5F5',
        height: '100vh',
        left: 0,
    };

    const contentStyle: React.CSSProperties = {
        textAlign: 'center',
        minHeight: 120,
        lineHeight: '20px',
        color: '#fff',
    };

    const footerStyle: React.CSSProperties = {
        textAlign: 'center',
        color: '#fff',
    };

    const layoutStyle = {
        borderRadius: 8,
        overflow: 'hidden',
        width: "100%",
        maxWidth: 1500,
    };

    const items: MenuProps['items'] = [
        { label: 'Dash board', key: '/admin/dash-board', icon: <DashboardOutlined /> },
        { label: 'Realm', key: '/admin/realm', icon: <ProductFilled /> },
        { label: 'Usert', key: '/admin/user', icon: <UserOutlined /> },
    ];

    return (
        <>
            <Flex gap="middle" wrap>
                <Layout style={layoutStyle}>
                    <Header style={headerStyle}>Header</Header>
                    <Layout>
                        <Sider width="20%" style={siderStyle}>
                            <Menu
                                style={{ display: 'block' }}
                                onClick={({ key }) => {
                                    if (key === 'sign-out') {
                                    } else {
                                        navigator(key);
                                    }
                                }}
                                items={items}
                            />
                        </Sider>
                        <Content style={contentStyle}>
                            <Routes>
                                <Route path='/realm' element={<ListRealm />} />
                                <Route path='/user' element={<ListUser />} />
                            </Routes>
                        </Content>
                    </Layout>
                    <Footer style={footerStyle}>Footer</Footer>
                </Layout>
            </Flex>
        </>
    )
}

export default LayoutAdmin