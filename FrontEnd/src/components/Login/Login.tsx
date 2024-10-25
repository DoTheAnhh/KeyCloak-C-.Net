import React, { useEffect, useState } from 'react';
import { Form, Input, Button, message } from 'antd';
import axios from 'axios';
import { LoginRequest } from '../../Interface/Login';
import { useNavigate } from 'react-router-dom';

const Login: React.FC = () => {
  const [loading, setLoading] = useState(false);
  const navigator = useNavigate();

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

      navigator("/user");

    } catch (error) {
      message.error('Đăng nhập thất bại! Kiểm tra lại thông tin.');
    } finally {
      setLoading(false);
    }
  };

  const onLoginByGoogle = () => {
    window.location.replace("https://localhost:44333/api/auth/redirect-to-google");
  };

  const onLoginByKeycloak = () => {
    window.location.replace("https://localhost:44333/api/auth/redirect-to-keycloak");
  };

  const onLoginByGithub = () => {
    window.location.replace("https://localhost:44333/api/auth/redirect-to-github");
  };

  const toRegister = () => {
    navigator("/register");
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

        <div style={{ textAlign: 'center', marginTop: 20 }}>
          <Button
            block
            style={{
              backgroundColor: 'gray',
              color: 'white',
              border: 'none',
              borderRadius: '4px',
              padding: '10px 20px',
              fontSize: '16px',
              fontWeight: 'bold',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'flex-start',
              transition: 'background-color 0.3s ease',
            }}
            onClick={onLoginByGoogle}
            onMouseEnter={(e) => (e.currentTarget.style.backgroundColor = '#357AE8')}
            onMouseLeave={(e) => (e.currentTarget.style.backgroundColor = '#4285F4')}
          >
            <img
              src="https://static-00.iconduck.com/assets.00/google-icon-2048x2048-pks9lbdv.png"
              alt="Google Logo"
              style={{ width: '20px', marginRight: '10px' }}
            />
            <div style={{ marginLeft: 50 }}>Đăng nhập bằng Google</div>
          </Button>
        </div>

        <div style={{ textAlign: 'center', marginTop: 20 }}>
          <Button
            block
            style={{
              backgroundColor: 'gray',
              color: 'white',
              border: 'none',
              borderRadius: '4px',
              padding: '10px 20px',
              fontSize: '16px',
              fontWeight: 'bold',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'flex-start',
              transition: 'background-color 0.3s ease',
            }}
            onClick={onLoginByGithub}
            onMouseEnter={(e) => (e.currentTarget.style.backgroundColor = '#357AE8')}
            onMouseLeave={(e) => (e.currentTarget.style.backgroundColor = '#4285F4')}
          >
            <img
              src="https://cdn-icons-png.flaticon.com/512/25/25231.png"
              alt="Keycloak Logo"
              style={{ width: '20px', marginRight: '10px' }}
            />
            <div style={{ marginLeft: 50 }}>Đăng nhập bằng Github</div>
          </Button>
        </div>

        <div style={{ textAlign: 'center', marginTop: 20 }}>
          <Button
            block
            style={{
              backgroundColor: 'gray',
              color: 'white',
              border: 'none',
              borderRadius: '4px',
              padding: '10px 20px',
              fontSize: '16px',
              fontWeight: 'bold',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'flex-start',
              transition: 'background-color 0.3s ease',
            }}
            onClick={onLoginByKeycloak}
            onMouseEnter={(e) => (e.currentTarget.style.backgroundColor = '#357AE8')}
            onMouseLeave={(e) => (e.currentTarget.style.backgroundColor = '#4285F4')}
          >
            <img
              src="https://upload.wikimedia.org/wikipedia/commons/2/29/Keycloak_Logo.png"
              alt="Keycloak Logo"
              style={{ width: '20px', marginRight: '10px' }}
            />
            <div style={{ marginLeft: 50 }}>Đăng nhập bằng Keycloak</div>
          </Button>
        </div>

        <a
          style={{
            display: 'inline-block',
            alignItems: 'center',
            color: '#1890ff',
            fontWeight: 'bold',
            fontSize: '14px',
            textDecoration: 'none',
            transition: 'color 0.3s ease',
            marginTop: 30,
            float: 'right',
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
