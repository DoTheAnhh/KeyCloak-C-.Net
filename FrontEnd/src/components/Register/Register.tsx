import React, { useState } from 'react';
import { Form, Input, Button, message } from 'antd';
import { RegisterRequest } from '../../Interface/Register';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';

const Register: React.FC = () => {

    const [loading, setLoading] = useState(false);

    const navigator = useNavigate()

    const onRegister = async (values: RegisterRequest) => {
        setLoading(true);
        try {
            const response = await axios.post('https://localhost:44333/api/auth/register', {
                username: values.username,
                password: values.password,
            });

            const { access_token, realm, refreshToken } = response.data;

            localStorage.setItem('token', access_token);
            localStorage.setItem('refreshToken', refreshToken);
            localStorage.setItem('realm', realm);
            
            message.success("Đăng ký thành công")

            navigator("/")
        } catch (error) {
            message.error('Đăng nhập thất bại! Kiểm tra lại thông tin.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div style={{ maxWidth: 400, margin: '0 auto', marginTop: 250 }}>
            <h2 style={{ paddingBottom: 50, fontWeight: 'bold', fontSize: 40, fontFamily: 'monospace', textAlign: 'center' }}>
                Đăng ký
            </h2>
            <Form onFinish={onRegister}>
                <Form.Item
                    name="username"
                    rules={[{ required: true, message: 'Vui lòng nhập tên đăng nhập!' }]}
                >
                    <Input placeholder="Tên đăng nhập" />
                </Form.Item>

                <Form.Item
                    name="password"
                    rules={[{ required: true, message: 'Vui lòng nhập mật khẩu!' }]}
                >
                    <Input.Password placeholder="Mật khẩu" />
                </Form.Item>

                <Form.Item>
                    <Button type="primary" htmlType="submit" loading={loading} block>
                        Đăng ký
                    </Button>
                </Form.Item>
            </Form>
        </div>
    )
}

export default Register