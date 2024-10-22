
const Success: React.FC = () => {

  const token = localStorage.getItem('token');

  return (
    <div>
      <div style={{marginBottom: 20}}>Token của bạn là</div>
      {token}
    </div>
  )
}

export default Success