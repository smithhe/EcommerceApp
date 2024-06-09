const LoadingIcon = () => {
    return (
        <div className="row w-100 text-center">
            <style>{`
            .loading-container {
              display: flex;
              flex-direction: column;
              justify-content: center;
              align-items: center;
              height: 100%;
            }
            .spinner-border {
              width: 5rem;
              height: 5rem;
              border-width: .5rem;
            }
            .loading-text {
              margin-top: 1rem;
              font-size: 1.5rem;
              font-weight: bold;
            }`}
            </style>
            <div className="col h-100">
                <div className="loading-container">
                    <div className="spinner-border text-primary" role="status">
                    </div>
                    <p className="loading-text">Loading...</p>
                </div>
            </div>
        </div>

    );
}

export default LoadingIcon;