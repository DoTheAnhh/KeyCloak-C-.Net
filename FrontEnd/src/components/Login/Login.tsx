import React, { useState } from 'react';
import { Form, Input, Button, message } from 'antd';
import axios from 'axios';
import { LoginRequest } from '../../Interface/Login';
import { useNavigate } from 'react-router-dom';

const Login: React.FC = () => {
  const [loading, setLoading] = useState(false);

  const navigator = useNavigate()

  const onLogin = async (values: LoginRequest) => {
    setLoading(true);
    try {
      const response = await axios.post('https://localhost:44333/api/auth/login', {
        username: values.username,
        password: values.password,
      });

      const { token, realm, refreshToken } = response.data;
      message.success('Đăng nhập thành công!');

      localStorage.setItem('token', token);
      localStorage.setItem('refreshToken', refreshToken);
      localStorage.setItem('realm', realm);

      navigator("/user")

    } catch (error) {
      message.error('Đăng nhập thất bại! Kiểm tra lại thông tin.');
    } finally {
      setLoading(false);
    }
  };

  const toRegister = () => {
    navigator("/register")
  }

  return (
    <div style={{ maxWidth: 400, margin: '0 auto', marginTop: 250 }}>
      <h2 style={{ paddingBottom: 50, fontWeight: 'bold', fontSize: 40, fontFamily: 'monospace', textAlign: 'center' }}>
        Đăng nhập
      </h2>
      <Form onFinish={onLogin}>
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
            Đăng nhập
          </Button>
        </Form.Item>

        <a
          style={{
            display: 'inline-block',
            alignItems: 'center',
            color: '#1890ff',
            fontWeight: 'bold',
            fontSize: '14px',
            textDecoration: 'none',
            transition: 'color 0.3s ease',
          }}
          onClick={toRegister}
          onMouseEnter={(e) => (e.currentTarget.style.color = '#40a9ff')}
          onMouseLeave={(e) => (e.currentTarget.style.color = '#1890ff')}
        >
          Đăng ký
        </a>
      </Form>
    </div>
  );
};

export default Login;
