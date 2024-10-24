import React, { useEffect, useState } from "react";
import { UserResponse } from "../../Interface/User";
import axios from "axios";
import { Table, message, Spin, Button } from "antd";
import { ColumnsType } from "antd/es/table";
import { useNavigate } from "react-router-dom";

const ListUser: React.FC = () => {
    const [users, setUsers] = useState<UserResponse[]>([]);
    const [loading, setLoading] = useState<boolean>(false);

    const navigator = useNavigate()

    const realm = localStorage.getItem("realm");
    const token = localStorage.getItem("token");
    const refreshToken = localStorage.getItem("refreshToken");

    const getUsers = async () => {
        if (!token || !realm) {
            message.error("Missing token or realm.");
            return;
        }
        setLoading(true);
        try {
            const res = await axios.get('https://localhost:44333/api/auth/users', {
                headers: {
                    Authorization: `Bearer ${token}`,
                },
            });
            setUsers(res.data);
        } catch (error) {
            console.error("Failed to fetch users:", error);
            message.error("Failed to fetch users. Please try again.");
        } finally {
            setLoading(false);
        }
    };

    const onLogout = async () => {
        await axios.post(`https://localhost:44333/api/auth/logout?refreshToken=${refreshToken}`)
        localStorage.removeItem("realm");
        localStorage.removeItem("token");
        localStorage.removeItem("refreshToken");
        navigator("/")
    }

    const columns: ColumnsType<UserResponse> = [
        {
            title: "Id",
            dataIndex: "id",
            key: "id",
            align: "center",
        },
        {
            title: "Username",
            dataIndex: "username",
            key: "username",
            align: "center",
        },
        {
            title: "Email",
            dataIndex: "email",
            key: "email",
            align: "center",
        },
        {
            title: "Enabled",
            dataIndex: "enabled",
            key: "enabled",
            align: "center",
            render: (text) => (text ? "Disabled" : "Enable"),
        }
    ];

    useEffect(() => {
        getUsers();
    }, []);
    return (
        <>
            {loading ? (
                <Spin size="large" />
            ) : (
                <Table dataSource={users} columns={columns} rowKey="id" pagination={false} />
            )}
            <Button type="primary" style={{ float: 'right', marginRight: 5 }} onClick={onLogout}>
                Đăng xuất
            </Button>
        </>
    );
};

export default ListUser;
