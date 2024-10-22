import React, { useEffect, useState } from 'react';
import { UserResponse } from '../../../Interface/User';
import axios from 'axios';
import { LOCALHOST, MAPPING_URL } from '../../../api/API';
import Table, { ColumnsType } from 'antd/es/table';
import { Button, Modal, Select, Space } from 'antd';
import { EditOutlined } from '@ant-design/icons';
import { RealmResponse } from '../../../Interface/Realm';
import UserForm from './UserForm';

const ListUser: React.FC = () => {
    const [users, setUsers] = useState<UserResponse[]>([]);
    const [realms, setRealms] = useState<RealmResponse[]>([]);
    const [isOpenUserModal, setIsOpenUserModal] = useState(false);
    const [userSelected, setUserSelected] = useState<string | null>(null);
    const [realmSelected, setRealmSelected] = useState<string | null>(null);

    const getAllUsers = async () => {
        if (!realmSelected) return;
        try {
            const res = await axios.get(LOCALHOST + MAPPING_URL.USER + `?realm=${realmSelected}`);
            setUsers(res.data);
        } catch (error) {
            console.error("Error fetching users:", error);
        }
    };

    const getAllRealms = async () => {
        try {
            const res = await axios.get(LOCALHOST + MAPPING_URL.REALM);
            setRealms(res.data);
        } catch (err) {
            console.error("Error fetching realms:", err);
        }
    };

    const showUserModal = () => {
        setUserSelected(null);
        setIsOpenUserModal(true);
    };

    const showUserModalById = (record: UserResponse) => {
        setUserSelected(record.id);
        setIsOpenUserModal(true);
    };

    const handleUserModalCancel = () => {
        setIsOpenUserModal(false);
        setUserSelected(null);
    };

    const columns: ColumnsType<UserResponse> = [
        {
            title: 'Id',
            dataIndex: 'id',
            key: 'id',
            align: 'center',
        },
        {
            title: 'User name',
            dataIndex: 'username',
            key: 'username',
            align: 'center',
        },
        {
            title: 'Action',
            key: 'action',
            align: 'center',
            render: (record: UserResponse) => (
                <Space size="middle">
                    <EditOutlined
                        style={{ color: '#1890ff', cursor: 'pointer' }}
                        onClick={() => showUserModalById(record)}
                    />
                </Space>
            ),
        },
    ];

    const handleRealmChange = (value: string) => {
        setRealmSelected(value);
    };

    useEffect(() => {
        getAllUsers();
    }, [realmSelected]);

    useEffect(() => {
        const fetchRealms = async () => {
            await getAllRealms();
            if (realms.length > 0) {
                const defaultRealm = realms[0].realm;
                setRealmSelected(defaultRealm);
            }
        };

        fetchRealms();
    }, []);

    return (
        <>
            <Modal
                open={isOpenUserModal}
                onCancel={handleUserModalCancel}
                footer={null}
            >
                <UserForm
                    realmSelected={realmSelected}
                    handleCancelUserModal={handleUserModalCancel}
                    userSelected={userSelected}
                />
            </Modal>
            <Button type='primary' style={{ float: 'right', margin: "10px" }} onClick={showUserModal}>
                New realm +
            </Button>
            <Select
                style={{ width: 200, margin: 20 }}
                placeholder="Select Realm"
                onChange={handleRealmChange}
                value={realmSelected}
            >
                {realms.map(realm => (
                    <Select.Option key={realm.realm} value={realm.realm}>
                        {realm.realm}
                    </Select.Option>
                ))}
            </Select>
            <Table<UserResponse>
                columns={columns}
                dataSource={users}
                rowKey="id"
                pagination={false}
            />
        </>
    );
}

export default ListUser;
